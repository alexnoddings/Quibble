using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Quibble.Server.Data.Models;
using Quibble.Server.Extensions;
using Quibble.Shared.Entities;
using Quibble.Shared.Hub;
using Quibble.Shared.Models;

namespace Quibble.Server.Hub
{
    public partial class QuibbleHub
    {
        [HubMethodName(Endpoints.CreateRound)]
        public async Task<HubResponse> CreateRoundAsync(string title)
        {
            (Guid userId, Guid quizId, ApiError? error) = ExecutionContext;
            if (error is not null)
                return Failure(error);

            var dbQuiz =
                await DbContext.Quizzes
                    .Include(quiz => quiz.Rounds)
                    .FindAsync(quizId);

            if (dbQuiz is null)
                return Failure(HubErrors.RoundParentQuizNotFound);

            if (dbQuiz.OwnerId != userId)
                return Failure(HubErrors.CantEditAsNotOwner);

            if (dbQuiz.State != QuizState.InDevelopment)
                return Failure(HubErrors.CantDeleteAsNotInDevelopment);

            title ??= string.Empty;
            if (title.Length > 100)
                return Failure(HubErrors.RoundMissingTitle);

            var dbRound = new DbRound
            {
                Title = title,
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
            (Guid userId, Guid quizId, ApiError? error) = ExecutionContext;
            if (error is not null)
                return Failure(error);

            var dbRound =
                await DbContext.Rounds
                    .Include(round => round.Quiz)
                    .FindAsync(roundId);

            if (dbRound is null)
                return Failure(HubErrors.RoundNotFound);

            if (dbRound.QuizId != quizId)
                return Failure(HubErrors.RoundNotFound);

            if (dbRound.Quiz.OwnerId != userId)
                return Failure(HubErrors.CantEditAsNotOwner);

            if (dbRound.Quiz.State != QuizState.InDevelopment)
                return Failure(HubErrors.CantDeleteAsNotInDevelopment);

            newTitle ??= string.Empty;
            if (newTitle.Length > 200)
                return Failure(HubErrors.RoundMissingTitle);

            dbRound.Title = newTitle;
            await DbContext.SaveChangesAsync();

            await AllQuizUsersGroup(quizId).OnRoundTitleUpdatedAsync(dbRound.Id, newTitle);

            return Success();
        }

        [HubMethodName(Endpoints.OpenRound)]
        public async Task<HubResponse> OpenRoundAsync(Guid roundId)
        {
            (Guid userId, Guid quizId, ApiError? error) = ExecutionContext;
            if (error is not null)
                return Failure(error);

            var dbRound =
                await DbContext.Rounds
                    .Include(round => round.Quiz)
                    .FindAsync(roundId);

            if (dbRound is null)
                return Failure(HubErrors.RoundNotFound);

            if (dbRound.QuizId != quizId)
                return Failure(HubErrors.RoundNotFound);

            if (dbRound.Quiz.OwnerId != userId)
                return Failure(HubErrors.CantEditAsNotOwner);

            if (dbRound.Quiz.State != QuizState.Open)
                return Failure(HubErrors.CantUpdateAsNotOpen);

            dbRound.State = RoundState.Open;
            await DbContext.SaveChangesAsync();

            await QuizHostGroup(quizId).OnRoundOpenedAsync(dbRound.Id);

            var round = Mapper.Map<RoundDto>(dbRound);
            await AllQuizParticipantsGroup(quizId).OnRoundAddedAsync(round);

            return Success();
        }

        [HubMethodName(Endpoints.DeleteRound)]
        public async Task<HubResponse> DeleteRoundAsync(Guid roundId)
        {
            (Guid userId, Guid quizId, ApiError? error) = ExecutionContext;
            if (error is not null)
                return Failure(error);

            var dbRound =
                await DbContext.Rounds
                    .Include(round => round.Quiz)
                    .FindAsync(roundId);

            if (dbRound is null)
                return Failure(HubErrors.RoundNotFound);

            if (dbRound.QuizId != quizId)
                return Failure(HubErrors.RoundNotFound);

            if (dbRound.Quiz.OwnerId != userId)
                return Failure(HubErrors.CantEditAsNotOwner);

            if (dbRound.Quiz.State != QuizState.InDevelopment)
                return Failure(HubErrors.CantDeleteAsNotInDevelopment);

            DbContext.Rounds.Remove(dbRound);
            await DbContext.SaveChangesAsync();

            await AllQuizUsersGroup(quizId).OnRoundDeletedAsync(dbRound.Id);

            return Success();
        }
    }
}
