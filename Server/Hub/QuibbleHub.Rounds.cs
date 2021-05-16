using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Quibble.Server.Data.Models;
using Quibble.Server.Extensions;
using Quibble.Shared.Entities;
using Quibble.Shared.Hub;
using Quibble.Shared.Models;
using Quibble.Shared.Resources;

namespace Quibble.Server.Hub
{
    public partial class QuibbleHub
    {
        [HubMethodName(Endpoints.CreateRound)]
        public async Task<HubResponse> CreateRoundAsync(string title)
        {
            (Guid userId, Guid quizId, string? errorCode) = ExecutionContext;
            if (errorCode is not null)
                return Failure(errorCode);

            var dbQuiz = 
                await DbContext.Quizzes
                    .Include(quiz => quiz.Rounds)
                    .FindAsync(quizId);

            if (dbQuiz is null)
                return Failure(nameof(ErrorMessages.RoundParentQuizNotFound));

            if (dbQuiz.OwnerId != userId)
                return Failure(nameof(ErrorMessages.QuizCantEditAsNotOwner));

            if (dbQuiz.State != QuizState.InDevelopment)
                return Failure(nameof(ErrorMessages.CantEditAsQuizNotInDevelopment));

            var dbRound = new DbRound
            {
                Title = title ?? string.Empty,
                State = RoundState.Hidden
            };
            dbQuiz.Rounds.Add(dbRound);
            await DbContext.SaveChangesAsync();

            var round = Mapper.Map<RoundDto>(dbRound);
            await AllQuizUsersGroup(quizId).OnRoundAddedAsync(round);

            return Success();
        }

        [HubMethodName(Endpoints.UpdateRoundTitle)]
        public async Task<HubResponse> UpdateRoundTitleAsync(Guid roundId, string newTitle)
        {
            (Guid userId, Guid quizId, string? errorCode) = ExecutionContext;
            if (errorCode is not null)
                return Failure(errorCode);

            var dbRound = 
                await DbContext.Rounds
                    .Include(round => round.Quiz)
                    .FindAsync(roundId);

            if (dbRound is null)
                return Failure(nameof(ErrorMessages.RoundNotFound));

            if (dbRound.QuizId != quizId)
                return Failure(nameof(ErrorMessages.RoundNotFound));

            if (dbRound.Quiz.OwnerId != userId)
                return Failure(nameof(ErrorMessages.QuizCantEditAsNotOwner));

            if (dbRound.Quiz.State != QuizState.InDevelopment)
                return Failure(nameof(ErrorMessages.CantEditAsQuizNotInDevelopment));

            newTitle ??= string.Empty;
            dbRound.Title = newTitle;
            await DbContext.SaveChangesAsync();

            await AllQuizUsersGroup(quizId).OnRoundTitleUpdatedAsync(dbRound.Id, newTitle);

            return Success();
        }

        [HubMethodName(Endpoints.OpenRound)]
        public async Task<HubResponse> OpenRoundAsync(Guid roundId)
        {
            (Guid userId, Guid quizId, string? errorCode) = ExecutionContext;
            if (errorCode is not null)
                return Failure(errorCode);

            var dbRound = 
                await DbContext.Rounds
                    .Include(round => round.Quiz)
                    .FindAsync(roundId);

            if (dbRound is null)
                return Failure(nameof(ErrorMessages.RoundNotFound));

            if (dbRound.QuizId != quizId)
                return Failure(nameof(ErrorMessages.RoundNotFound));

            if (dbRound.Quiz.OwnerId != userId)
                return Failure(nameof(ErrorMessages.QuizCantEditAsNotOwner));

            if (dbRound.Quiz.State != QuizState.Open)
                return Failure(nameof(ErrorMessages.CantUpdateAsQuizNotOpen));

            dbRound.State = RoundState.Open;
            await DbContext.SaveChangesAsync();

            await AllQuizUsersGroup(quizId).OnRoundOpenedAsync(dbRound.Id);

            return Success();
        }

        [HubMethodName(Endpoints.DeleteRound)]
        public async Task<HubResponse> DeleteRoundAsync(Guid roundId)
        {
            (Guid userId, Guid quizId, string? errorCode) = ExecutionContext;
            if (errorCode is not null)
                return Failure(errorCode);

            var dbRound = 
                await DbContext.Rounds
                    .Include(round => round.Quiz)
                    .FindAsync(roundId);

            if (dbRound is null)
                return Failure(nameof(ErrorMessages.RoundNotFound));

            if (dbRound.QuizId != quizId)
                return Failure(nameof(ErrorMessages.RoundNotFound));

            if (dbRound.Quiz.OwnerId != userId)
                return Failure(nameof(ErrorMessages.QuizCantEditAsNotOwner));

            if (dbRound.Quiz.State != QuizState.InDevelopment)
                return Failure(nameof(ErrorMessages.CantEditAsQuizNotInDevelopment));

            DbContext.Rounds.Remove(dbRound);
            await DbContext.SaveChangesAsync();

            await AllQuizUsersGroup(quizId).OnRoundDeletedAsync(dbRound.Id);

            return Success();
        }
    }
}
