using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quibble.Common.Api;
using Quibble.Common.Dtos;
using Quibble.Common.Entities;
using Quibble.Server.Core;
using Quibble.Server.Core.Models;
using Quibble.Server.Core.Sync;

namespace Quibble.Server.Core.Domain;

public class QuizLogic : BaseLogic
{
	public QuizLogic(ILogger<QuizLogic> logger, IMapper mapper, AppDbContext dbContext)
		: base(logger, mapper, dbContext)
	{
	}

	public async Task<bool> OnUserConnectedAsync(IExecutionContext context)
	{
		var dbQuiz = await DbContext.Quizzes
			.Include(quiz => quiz.Participants)
			.FindAsync(context.QuizId);

		if (dbQuiz is null)
			return false;

		if (dbQuiz.OwnerId == context.UserId)
			await context.Users.AddCurrentUserAsHost();
		else
		{
			if (dbQuiz.State != QuizState.Open)
				return false;

			DbParticipant? dbParticipant = dbQuiz.Participants.Find(participant => participant.UserId == context.UserId);
			if (dbParticipant is null)
			{
				var dbUser = (await DbContext.Users.FindAsync(context.UserId))!;
				dbParticipant = new DbParticipant { Quiz = dbQuiz, User = dbUser };
				dbQuiz.Participants.Add(dbParticipant);
				await DbContext.SaveChangesAsync();

				// Only add answers for questions which haven't been locked yet
				var dbHiddenAndUnlockedQuestionIdsQueryable =
					from question in DbContext.Questions
					join round in DbContext.Rounds
						on question.RoundId equals round.Id
					where round.QuizId == dbQuiz.Id
						  && question.State < QuestionState.Locked
					select question.Id;
				var dbSubmittedAnswersQueryable =
					from questionId in dbHiddenAndUnlockedQuestionIdsQueryable
					select new DbSubmittedAnswer { QuestionId = questionId, Participant = dbParticipant, AssignedPoints = -1, Text = string.Empty };

				var dbSubmittedAnswers = await dbSubmittedAnswersQueryable.ToListAsync();
				DbContext.SubmittedAnswers.AddRange(dbSubmittedAnswers);
				await DbContext.SaveChangesAsync();

				var participantDto = Mapper.Map<ParticipantDto>(dbParticipant);
				var submittedAnswerDtos = Mapper.Map<List<SubmittedAnswerDto>>(dbSubmittedAnswers);

				await context.Users.Hosts().OnParticipantJoinedAsync(participantDto, submittedAnswerDtos);
				await context.Users.Participants().OnParticipantJoinedAsync(participantDto, new List<SubmittedAnswerDto>());
			}

			await context.Users.AddCurrentUserAsParticipant(dbParticipant.Id);
		}

		return true;
	}

	public async Task<ApiResponse<FullQuizDto>> GetQuizAsync(IExecutionContext context)
	{
		var dbQuiz =
			await DbContext.Quizzes
				.Include(q => q.Participants)
					.ThenInclude(pt => pt.User)
				.Include(q => q.Rounds)
					.ThenInclude(r => r.Questions)
						.ThenInclude(qs => qs.SubmittedAnswers)
				.FindAsync(context.QuizId);

		if (dbQuiz is null)
			return Failure<FullQuizDto>(ApiErrors.QuizNotFound);

		if (dbQuiz.OwnerId != context.UserId && dbQuiz.State != QuizState.Open)
			return Failure<FullQuizDto>(ApiErrors.QuizNotOpen);

		var quiz = Mapper.Map<QuizDto>(dbQuiz);
		List<RoundDto> rounds;
		List<ParticipantDto> participants;
		List<QuestionDto> questions;
		List<SubmittedAnswerDto> submittedAnswers;

		if (dbQuiz.State == QuizState.InDevelopment)
		{
			rounds = Mapper.Map<List<RoundDto>>(dbQuiz.Rounds);
			participants = new List<ParticipantDto>();
			questions = Mapper.Map<List<QuestionDto>>(dbQuiz.Rounds.SelectMany(r => r.Questions));
			submittedAnswers = new List<SubmittedAnswerDto>();
		}
		else if (dbQuiz.State == QuizState.Open && dbQuiz.OwnerId == context.UserId)
		{
			rounds = Mapper.Map<List<RoundDto>>(dbQuiz.Rounds);
			participants = Mapper.Map<List<ParticipantDto>>(dbQuiz.Participants);

			var dbQuestions = dbQuiz.Rounds.SelectMany(r => r.Questions).ToList();
			questions = Mapper.Map<List<QuestionDto>>(dbQuestions);

			var dbSubmittedAnswers = dbQuestions.SelectMany(qs => qs.SubmittedAnswers).ToList();
			submittedAnswers = Mapper.Map<List<SubmittedAnswerDto>>(dbSubmittedAnswers);
		}
		else
		{
			var dbParticipant = await DbContext.Participants.FirstAsync(p => p.QuizId == context.QuizId && p.UserId == context.UserId);

			var visibleDbRounds = dbQuiz.Rounds.Where(r => r.State == RoundState.Open).ToList();
			rounds = Mapper.Map<List<RoundDto>>(visibleDbRounds);
			participants = Mapper.Map<List<ParticipantDto>>(dbQuiz.Participants);
			// Mark that user's participant
			participants.Single(participant => participant.Id == dbParticipant.Id).IsCurrentUser = true;

			var visibleDbQuestions = visibleDbRounds.SelectMany(r => r.Questions).Where(q => q.State != QuestionState.Hidden).ToList();
			questions = Mapper.Map<List<QuestionDto>>(visibleDbQuestions);

			var userDbSubmittedAnswers = visibleDbQuestions.SelectMany(q => q.SubmittedAnswers).Where(sa => sa.ParticipantId == dbParticipant.Id).ToList();
			submittedAnswers = Mapper.Map<List<SubmittedAnswerDto>>(userDbSubmittedAnswers);
		}

		return Success(new FullQuizDto(quiz, participants, rounds, questions, submittedAnswers));
	}

	public async Task<ApiResponse> OpenQuizAsync(IExecutionContext context)
	{
		var dbQuiz =
			await DbContext.Quizzes
				.Include(q => q.Rounds)
					.ThenInclude(r => r.Questions)
				.FindAsync(context.QuizId);

		if (dbQuiz is null)
			return Failure(ApiErrors.QuizNotFound);

		if (dbQuiz.OwnerId != context.UserId)
			return Failure(ApiErrors.CantEditAsNotOwner);

		if (dbQuiz.State == QuizState.Open)
			return Failure(ApiErrors.QuizAlreadyOpen);

		if (!dbQuiz.Rounds.SelectMany(round => round.Questions).Any())
			return Failure(ApiErrors.QuizEmpty);

		if (dbQuiz.Rounds.Any(round => string.IsNullOrWhiteSpace(round.Title)))
			return Failure(ApiErrors.RoundMissingTitle);

		if (dbQuiz.Rounds.SelectMany(round => round.Questions).Any(question => string.IsNullOrWhiteSpace(question.Text)))
			return Failure(ApiErrors.QuestionMissingText);

		if (dbQuiz.Rounds.SelectMany(round => round.Questions).Any(question => string.IsNullOrWhiteSpace(question.Answer)))
			return Failure(ApiErrors.QuestionMissingAnswer);

		dbQuiz.State = QuizState.Open;
		dbQuiz.OpenedAt = DateTime.UtcNow;

		var roundIndex = 0;
		foreach (var round in dbQuiz.Rounds)
		{
			round.Order = roundIndex++;
			var questionIndex = 0;
			foreach (var question in round.Questions)
				question.Order = questionIndex++;
		}

		await DbContext.SaveChangesAsync();

		await context.Users.All().OnQuizOpenedAsync();

		return Success();
	}

	public async Task<ApiResponse> UpdateQuizTitleAsync(IExecutionContext context, string newTitle)
	{
		var dbQuiz = await DbContext.Quizzes.FindAsync(context.QuizId);
		if (dbQuiz is null)
			return Failure(ApiErrors.QuizNotFound);

		if (dbQuiz.OwnerId != context.UserId)
			return Failure(ApiErrors.CantEditAsNotOwner);

		if (dbQuiz.State != QuizState.InDevelopment)
			return Failure(ApiErrors.CantDeleteAsNotInDevelopment);

		newTitle ??= string.Empty;
		if (newTitle.Length > 100)
			return Failure(ApiErrors.QuizEmpty);

		dbQuiz.Title = newTitle;
		await DbContext.SaveChangesAsync();

		await context.Users.All().OnQuizTitleUpdatedAsync(newTitle);

		return Success();
	}

	public async Task<ApiResponse> DeleteQuizAsync(IExecutionContext context)
	{
		var dbQuiz = await DbContext.Quizzes.FindAsync(context.QuizId);
		if (dbQuiz is null)
			return Failure(ApiErrors.QuizNotFound);

		if (dbQuiz.OwnerId != context.UserId)
			return Failure(ApiErrors.CantEditAsNotOwner);

		if (dbQuiz.State != QuizState.InDevelopment)
			return Failure(ApiErrors.CantDeleteAsNotInDevelopment);

		DbContext.Quizzes.Remove(dbQuiz);
		await DbContext.SaveChangesAsync();

		await context.Users.All().OnQuizDeletedAsync();

		return Success();
	}
}
