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

public class QuestionLogic : BaseLogic
{
	public QuestionLogic(ILogger<QuestionLogic> logger, IMapper mapper, AppDbContext dbContext)
		: base(logger, mapper, dbContext)
	{
	}

	public async Task<ApiResponse> CreateQuestionAsync(IExecutionContext context, Guid roundId)
	{
		var dbRound =
			await DbContext.Rounds
				.Include(round => round.Quiz)
				.Include(round => round.Questions)
				.FindAsync(roundId);

		if (dbRound is null)
			return Failure(ApiErrors.QuestionParentRoundNotFound);

		if (dbRound.QuizId != context.QuizId)
			return Failure(ApiErrors.QuestionParentRoundNotFound);

		if (dbRound.Quiz.OwnerId != context.UserId)
			return Failure(ApiErrors.CantEditAsNotOwner);

		if (dbRound.Quiz.State != QuizState.InDevelopment)
			return Failure(ApiErrors.CantDeleteAsNotInDevelopment);

		var dbQuestion = new DbQuestion
		{
			Text = string.Empty,
			Answer = string.Empty,
			Points = 1,
			State = QuestionState.Hidden,
			Order = dbRound.Questions.Count
		};
		dbRound.Questions.Add(dbQuestion);
		await DbContext.SaveChangesAsync();

		var question = Mapper.Map<QuestionDto>(dbQuestion);
		await context.Users.All().OnQuestionAddedAsync(question);

		return Success();
	}

	public async Task<ApiResponse> UpdateQuestionTextAsync(IExecutionContext context, Guid questionId, string newText)
	{
		var dbQuestion =
			await DbContext.Questions
				.Include(question => question.Round)
					.ThenInclude(round => round.Quiz)
				.FindAsync(questionId);

		if (dbQuestion is null)
			return Failure(ApiErrors.QuestionNotFound);

		if (dbQuestion.Round.QuizId != context.QuizId)
			return Failure(ApiErrors.QuestionNotFound);

		if (dbQuestion.Round.Quiz.OwnerId != context.UserId)
			return Failure(ApiErrors.CantEditAsNotOwner);

		if (dbQuestion.Round.Quiz.State != QuizState.InDevelopment)
			return Failure(ApiErrors.CantDeleteAsNotInDevelopment);

		newText ??= string.Empty;
		if (newText.Length > 200)
			return Failure(ApiErrors.TextTooLong);

		dbQuestion.Text = newText;
		await DbContext.SaveChangesAsync();

		await context.Users.All().OnQuestionTextUpdatedAsync(questionId, newText);

		return Success();
	}

	public async Task<ApiResponse> UpdateQuestionAnswerAsync(IExecutionContext context, Guid questionId, string newAnswer)
	{
		var dbQuestion =
			await DbContext.Questions
				.Include(question => question.Round)
					.ThenInclude(round => round.Quiz)
				.FindAsync(questionId);

		if (dbQuestion is null)
			return Failure(ApiErrors.QuestionNotFound);

		if (dbQuestion.Round.QuizId != context.QuizId)
			return Failure(ApiErrors.QuestionNotFound);

		if (dbQuestion.Round.Quiz.OwnerId != context.UserId)
			return Failure(ApiErrors.CantEditAsNotOwner);

		if (dbQuestion.Round.Quiz.State != QuizState.InDevelopment)
			return Failure(ApiErrors.CantDeleteAsNotInDevelopment);

		newAnswer ??= string.Empty;
		if (newAnswer.Length > 200)
			return Failure(ApiErrors.TextTooLong);

		dbQuestion.Answer = newAnswer;
		await DbContext.SaveChangesAsync();

		await context.Users.All().OnQuestionAnswerUpdatedAsync(questionId, newAnswer);

		return Success();
	}

	public async Task<ApiResponse> UpdateQuestionPointsAsync(IExecutionContext context, Guid questionId, decimal newPoints)
	{
		var dbQuestion =
			await DbContext.Questions
				.Include(question => question.Round)
					.ThenInclude(round => round.Quiz)
				.FindAsync(questionId);

		if (dbQuestion is null)
			return Failure(ApiErrors.QuestionNotFound);

		if (dbQuestion.Round.QuizId != context.QuizId)
			return Failure(ApiErrors.QuestionNotFound);

		if (dbQuestion.Round.Quiz.OwnerId != context.UserId)
			return Failure(ApiErrors.CantEditAsNotOwner);

		if (dbQuestion.Round.Quiz.State != QuizState.InDevelopment)
			return Failure(ApiErrors.CantDeleteAsNotInDevelopment);

		if (newPoints < 0.25m)
			return Failure(ApiErrors.PointsTooLow);

		if (newPoints > 10)
			return Failure(ApiErrors.PointsTooHigh);

		// Ensure points are a division of 0.25
		newPoints = Math.Round(newPoints * 4, MidpointRounding.ToEven) / 4;

		dbQuestion.Points = newPoints;
		await DbContext.SaveChangesAsync();

		await context.Users.All().OnQuestionPointsUpdatedAsync(questionId, newPoints);

		return Success();
	}

	public async Task<ApiResponse> UpdateQuestionStateAsync(IExecutionContext context, Guid questionId, QuestionState newState)
	{
		var dbQuestion =
			await DbContext.Questions
				.Include(question => question.Round)
					.ThenInclude(round => round.Quiz)
				.FindAsync(questionId);

		if (dbQuestion is null)
			return Failure(ApiErrors.QuestionNotFound);

		if (dbQuestion.Round.QuizId != context.QuizId)
			return Failure(ApiErrors.QuestionNotFound);

		if (dbQuestion.Round.Quiz.OwnerId != context.UserId)
			return Failure(ApiErrors.CantEditAsNotOwner);

		if (dbQuestion.Round.Quiz.State != QuizState.Open)
			return Failure(ApiErrors.CantUpdateAsNotOpen);

		if (dbQuestion.Round.State != RoundState.Open)
			return Failure(ApiErrors.QuestionBadState);

		// ToDo: split into multiple state-change functions
		switch (dbQuestion.State)
		{
			// The only valid state transitions
			case QuestionState.Hidden when newState == QuestionState.Open:
			case QuestionState.Open when newState == QuestionState.Locked:
			case QuestionState.Locked when newState == QuestionState.AnswerRevealed:
				break;
			// Invalid new state value or invalid state transitions
			default:
				return Failure(ApiErrors.QuestionBadState);
		}

		if (newState == QuestionState.AnswerRevealed)
		{
			var areAnyAnswersUnmarked = await DbContext.SubmittedAnswers
				.Where(submittedAnswer => submittedAnswer.QuestionId == dbQuestion.Id)
				.AnyAsync(submittedAnswer => submittedAnswer.AssignedPoints == -1);

			if (areAnyAnswersUnmarked)
				return Failure(ApiErrors.QuestionBadState);
		}

		dbQuestion.State = newState;
		await DbContext.SaveChangesAsync();

		if (newState == QuestionState.Open)
		{
			await context.Users.Hosts().OnQuestionStateUpdatedAsync(questionId, newState);

			var questionDto = Mapper.Map<QuestionDto>(dbQuestion);

			var submittedAnswers = await DbContext.SubmittedAnswers
				.Where(submittedAnswer => submittedAnswer.QuestionId == dbQuestion.Id)
				.ToListAsync();

			foreach (var submittedAnswer in submittedAnswers)
			{
				var submittedAnswerDto = Mapper.Map<SubmittedAnswerDto>(submittedAnswer);
				await context.Users.Participant(submittedAnswer.ParticipantId).OnQuestionRevealedAsync(questionDto, new List<SubmittedAnswerDto> { submittedAnswerDto });
			}

			return Success();
		}

		if (newState == QuestionState.Locked)
		{
			var emptySubmittedAnswers = await DbContext.SubmittedAnswers
				.Where(submittedAnswer => submittedAnswer.QuestionId == dbQuestion.Id)
				.Where(submittedAnswer => string.IsNullOrWhiteSpace(submittedAnswer.Text))
				.ToListAsync();

			foreach (var submittedAnswer in emptySubmittedAnswers)
				submittedAnswer.AssignedPoints = 0;

			await DbContext.SaveChangesAsync();

			foreach (var submittedAnswer in emptySubmittedAnswers)
				await context.Users.Hosts().OnSubmittedAnswerAssignedPointsUpdatedAsync(submittedAnswer.Id, 0);

			var answerLowerTrimmed = dbQuestion.Answer.Trim().ToLower();
			var correctSubmittedAnswers = await DbContext.SubmittedAnswers
				.Where(submittedAnswer => submittedAnswer.QuestionId == dbQuestion.Id)
				.Where(submittedAnswer => submittedAnswer.Text.Trim().ToLower() == answerLowerTrimmed)
				.ToListAsync();

			foreach (var submittedAnswer in correctSubmittedAnswers)
				submittedAnswer.AssignedPoints = dbQuestion.Points;

			await DbContext.SaveChangesAsync();

			foreach (var submittedAnswer in correctSubmittedAnswers)
				await context.Users.Hosts().OnSubmittedAnswerAssignedPointsUpdatedAsync(submittedAnswer.Id, submittedAnswer.AssignedPoints);
		}

		if (newState == QuestionState.AnswerRevealed)
		{
			var submittedAnswers = await DbContext.SubmittedAnswers
				.Where(submittedAnswer => submittedAnswer.QuestionId == dbQuestion.Id)
				.ToListAsync();

			foreach (var submittedAnswer in submittedAnswers)
				await context.Users.Participant(submittedAnswer.ParticipantId).OnSubmittedAnswerAssignedPointsUpdatedAsync(submittedAnswer.Id, submittedAnswer.AssignedPoints);
		}

		await context.Users.All().OnQuestionStateUpdatedAsync(questionId, newState);

		return Success();
	}

	public async Task<ApiResponse> DeleteQuestionAsync(IExecutionContext context, Guid questionId)
	{
		var dbQuestion =
			await DbContext.Questions
				.Include(question => question.Round)
					.ThenInclude(round => round.Quiz)
				.Include(question => question.Round)
					.ThenInclude(round => round.Questions)
				.FindAsync(questionId);

		if (dbQuestion is null)
			return Failure(ApiErrors.QuestionNotFound);

		if (dbQuestion.Round.QuizId != context.QuizId)
			return Failure(ApiErrors.QuestionNotFound);

		if (dbQuestion.Round.Quiz.OwnerId != context.UserId)
			return Failure(ApiErrors.CantEditAsNotOwner);

		if (dbQuestion.Round.Quiz.State != QuizState.InDevelopment)
			return Failure(ApiErrors.CantDeleteAsNotInDevelopment);

		DbContext.Questions.Remove(dbQuestion);
		dbQuestion.Round.Questions.Remove(dbQuestion);

		List<DbQuestion> modifiedQuestions = new(dbQuestion.Round.Questions.Count);
		var questionCount = 0;
		foreach (var roundQuestion in dbQuestion.Round.Questions.OrderBy(round => round.Order))
		{
			if (roundQuestion.Order != questionCount)
			{
				roundQuestion.Order = questionCount;
				modifiedQuestions.Add(roundQuestion);
			}
			questionCount++;
		}
		await DbContext.SaveChangesAsync();

		var allQuizUsersGroup = context.Users.All();
		await allQuizUsersGroup.OnQuestionDeletedAsync(dbQuestion.Id);
		foreach (var modifiedQuestion in modifiedQuestions)
			await allQuizUsersGroup.OnQuestionOrderUpdatedAsync(modifiedQuestion.Id, modifiedQuestion.Order);

		return Success();
	}
}
