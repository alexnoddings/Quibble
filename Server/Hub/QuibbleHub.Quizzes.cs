using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Quibble.Server.Extensions;
using Quibble.Shared.Api;
using Quibble.Shared.Entities;
using Quibble.Shared.Models.Dtos;
using Quibble.Shared.Sync.SignalR;

namespace Quibble.Server.Hub;

public partial class QuibbleHub
{
    [HubMethodName(SignalrEndpoints.GetQuiz)]
    public async Task<ApiResponse<FullQuizDto>> GetQuizAsync()
    {
        (Guid userId, Guid quizId, ApiError? error) = ExecutionContext;
        if (error is not null)
            return Failure<FullQuizDto>(error);

        var dbQuiz =
            await DbContext.Quizzes
                .Include(q => q.Participants)
                    .ThenInclude(pt => pt.User)
                .Include(q => q.Rounds)
                    .ThenInclude(r => r.Questions)
                        .ThenInclude(qs => qs.SubmittedAnswers)
                .FindAsync(quizId);

        if (dbQuiz is null)
            return Failure<FullQuizDto>(ApiErrors.QuizNotFound);

        if (dbQuiz.OwnerId != userId && dbQuiz.State != QuizState.Open)
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
        else if (dbQuiz.State == QuizState.Open && dbQuiz.OwnerId == userId)
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
            var dbParticipant = await DbContext.Participants.FirstAsync(p => p.QuizId == quizId && p.UserId == userId);

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

    [HubMethodName(SignalrEndpoints.UpdateQuizTitle)]
    public async Task<ApiResponse> UpdateQuizTitleAsync(string newTitle)
    {
        (Guid userId, Guid quizId, ApiError? error) = ExecutionContext;
        if (error is not null)
            return Failure(error);

        var dbQuiz = await DbContext.Quizzes.FindAsync(quizId);
        if (dbQuiz is null)
            return Failure(ApiErrors.QuizNotFound);

        if (dbQuiz.OwnerId != userId)
            return Failure(ApiErrors.CantEditAsNotOwner);

        if (dbQuiz.State != QuizState.InDevelopment)
            return Failure(ApiErrors.CantDeleteAsNotInDevelopment);

        newTitle ??= string.Empty;
        if (newTitle.Length > 100)
            return Failure(ApiErrors.QuizEmpty);

        dbQuiz.Title = newTitle;
        await DbContext.SaveChangesAsync();

        await AllQuizUsersGroup(quizId).OnQuizTitleUpdatedAsync(newTitle);

        return Success();
    }

    [HubMethodName(SignalrEndpoints.OpenQuiz)]
    public async Task<ApiResponse> OpenQuizAsync()
    {
        (Guid userId, Guid quizId, ApiError? error) = ExecutionContext;
        if (error is not null)
            return Failure(error);

        var dbQuiz =
            await DbContext.Quizzes
                .Include(q => q.Rounds)
                    .ThenInclude(r => r.Questions)
                .FindAsync(quizId);

        if (dbQuiz is null)
            return Failure(ApiErrors.QuizNotFound);

        if (dbQuiz.OwnerId != userId)
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

        int roundIndex = 0;
        foreach (var round in dbQuiz.Rounds)
        {
            round.Order = roundIndex++;
            int questionIndex = 0;
            foreach (var question in round.Questions)
            {
                question.Order = questionIndex++;
            }
        }

        await DbContext.SaveChangesAsync();

        await AllQuizUsersGroup(quizId).OnQuizOpenedAsync();

        return Success();
    }

    [HubMethodName(SignalrEndpoints.DeleteQuiz)]
    public async Task<ApiResponse> DeleteQuizAsync()
    {
        (Guid userId, Guid quizId, ApiError? error) = ExecutionContext;
        if (error is not null)
            return Failure(error);

        var dbQuiz = await DbContext.Quizzes.FindAsync(quizId);
        if (dbQuiz is null)
            return Failure(ApiErrors.QuizNotFound);

        if (dbQuiz.OwnerId != userId)
            return Failure(ApiErrors.CantEditAsNotOwner);

        if (dbQuiz.State != QuizState.InDevelopment)
            return Failure(ApiErrors.CantDeleteAsNotInDevelopment);

        DbContext.Quizzes.Remove(dbQuiz);
        await DbContext.SaveChangesAsync();

        await AllQuizUsersGroup(quizId).OnQuizDeletedAsync();

        return Success();
    }
}
