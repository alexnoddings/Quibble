using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Quibble.Server.Data.Models;
using Quibble.Server.Extensions;
using Quibble.Shared.Api;
using Quibble.Shared.Entities;
using Quibble.Shared.Models.Dtos;
using Quibble.Shared.Sync.SignalR;

namespace Quibble.Server.Hub
{
    public partial class QuibbleHub
    {
        [HubMethodName(SignalrEndpoints.CreateRound)]
        public async Task<ApiResponse> CreateRoundAsync()
        {
            (Guid userId, Guid quizId, ApiError? error) = ExecutionContext;
            if (error is not null)
                return Failure(error);

            var dbQuiz =
                await DbContext.Quizzes
                    .Include(quiz => quiz.Rounds)
                    .FindAsync(quizId);

            if (dbQuiz is null)
                return Failure(ApiErrors.RoundParentQuizNotFound);

            if (dbQuiz.OwnerId != userId)
                return Failure(ApiErrors.CantEditAsNotOwner);

            if (dbQuiz.State != QuizState.InDevelopment)
                return Failure(ApiErrors.CantDeleteAsNotInDevelopment);

            var dbRound = new DbRound
            {
                Title = string.Empty,
                State = RoundState.Hidden,
                Order = dbQuiz.Rounds.Count
            };
            dbQuiz.Rounds.Add(dbRound);
            await DbContext.SaveChangesAsync();

            var round = Mapper.Map<RoundDto>(dbRound);
            await AllQuizUsersGroup(quizId).OnRoundAddedAsync(round);

            return Success();
        }

        [HubMethodName(SignalrEndpoints.UpdateRoundTitle)]
        public async Task<ApiResponse> UpdateRoundTitleAsync(Guid roundId, string newTitle)
        {
            (Guid userId, Guid quizId, ApiError? error) = ExecutionContext;
            if (error is not null)
                return Failure(error);

            var dbRound =
                await DbContext.Rounds
                    .Include(round => round.Quiz)
                    .FindAsync(roundId);

            if (dbRound is null)
                return Failure(ApiErrors.RoundNotFound);

            if (dbRound.QuizId != quizId)
                return Failure(ApiErrors.RoundNotFound);

            if (dbRound.Quiz.OwnerId != userId)
                return Failure(ApiErrors.CantEditAsNotOwner);

            if (dbRound.Quiz.State != QuizState.InDevelopment)
                return Failure(ApiErrors.CantDeleteAsNotInDevelopment);

            newTitle ??= string.Empty;
            if (newTitle.Length > 100)
                return Failure(ApiErrors.RoundTitleTooLong);

            dbRound.Title = newTitle;
            await DbContext.SaveChangesAsync();

            await AllQuizUsersGroup(quizId).OnRoundTitleUpdatedAsync(dbRound.Id, newTitle);

            return Success();
        }

        [HubMethodName(SignalrEndpoints.OpenRound)]
        public async Task<ApiResponse> OpenRoundAsync(Guid roundId)
        {
            (Guid userId, Guid quizId, ApiError? error) = ExecutionContext;
            if (error is not null)
                return Failure(error);

            var dbRound =
                await DbContext.Rounds
                    .Include(round => round.Quiz)
                    .FindAsync(roundId);

            if (dbRound is null)
                return Failure(ApiErrors.RoundNotFound);

            if (dbRound.QuizId != quizId)
                return Failure(ApiErrors.RoundNotFound);

            if (dbRound.Quiz.OwnerId != userId)
                return Failure(ApiErrors.CantEditAsNotOwner);

            if (dbRound.Quiz.State != QuizState.Open)
                return Failure(ApiErrors.CantUpdateAsNotOpen);

            dbRound.State = RoundState.Open;
            await DbContext.SaveChangesAsync();

            await QuizHostGroup(quizId).OnRoundOpenedAsync(dbRound.Id);

            var round = Mapper.Map<RoundDto>(dbRound);
            await AllQuizParticipantsGroup(quizId).OnRoundAddedAsync(round);

            return Success();
        }

        [HubMethodName(SignalrEndpoints.DeleteRound)]
        public async Task<ApiResponse> DeleteRoundAsync(Guid roundId)
        {
            (Guid userId, Guid quizId, ApiError? error) = ExecutionContext;
            if (error is not null)
                return Failure(error);

            var dbRound =
                await DbContext.Rounds
                    .Include(round => round.Quiz)
                        .ThenInclude(quiz => quiz.Rounds)
                    .FindAsync(roundId);

            if (dbRound is null)
                return Failure(ApiErrors.RoundNotFound);

            if (dbRound.QuizId != quizId)
                return Failure(ApiErrors.RoundNotFound);

            if (dbRound.Quiz.OwnerId != userId)
                return Failure(ApiErrors.CantEditAsNotOwner);

            if (dbRound.Quiz.State != QuizState.InDevelopment)
                return Failure(ApiErrors.CantDeleteAsNotInDevelopment);

            DbContext.Rounds.Remove(dbRound);
            dbRound.Quiz.Rounds.Remove(dbRound);

            List<DbRound> modifiedRounds = new(dbRound.Quiz.Rounds.Count);
            int roundCount = 0;
            foreach (var quizRound in dbRound.Quiz.Rounds.OrderBy(round => round.Order))
            {
                if (quizRound.Order != roundCount)
                {
                    quizRound.Order = roundCount;
                    modifiedRounds.Add(quizRound);
                }
                roundCount++;
            }
            await DbContext.SaveChangesAsync();

            var allQuizUsersGroup = AllQuizUsersGroup(quizId);
            await allQuizUsersGroup.OnRoundDeletedAsync(dbRound.Id);
            foreach (var modifiedRound in modifiedRounds)
                await allQuizUsersGroup.OnRoundOrderUpdatedAsync(modifiedRound.Id, modifiedRound.Order);

            return Success();
        }
    }
}
