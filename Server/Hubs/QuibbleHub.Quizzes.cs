using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Quibble.Shared.Api;
using Quibble.Shared.Models;
using Quibble.Shared.Resources;

namespace Quibble.Server.Hubs
{
    public partial class QuibbleHub
    {
        [HubMethodName("Get")]
        public async Task<HubResponse<Quiz>> GetQuizAsync()
        {
            (Guid userId, Guid quizId, string? errorCode) = ExecutionContext;
            if (errorCode is not null)
                return Failure<Quiz>(errorCode);

            var dbQuiz = await DbContext.Quizzes.FindAsync(quizId);
            if (dbQuiz is null)
                return Failure<Quiz>(nameof(ErrorMessages.QuizNotFound));

            if (dbQuiz.OwnerId == userId)
                return Success(Mapper.Map<Quiz>(dbQuiz));

            if (dbQuiz.State == QuizState.Open)
                return Success(Mapper.Map<Quiz>(dbQuiz));

            return Failure<Quiz>(nameof(ErrorMessages.QuizNotOpen));
        }

        [HubMethodName("UpdateQuizTitle")]
        public async Task<HubResponse> UpdateQuizTitleAsync(string newTitle)
        {
            (Guid userId, Guid quizId, string? errorCode) = ExecutionContext;
            if (errorCode is not null)
                return Failure(errorCode);

            var dbQuiz = await DbContext.Quizzes.FindAsync(quizId);
            if (dbQuiz is null)
                return Failure(nameof(ErrorMessages.QuizNotFound));

            if (dbQuiz.OwnerId != userId)
                return Failure(nameof(ErrorMessages.QuizCantEditAsNotOwner));

            if (dbQuiz.State != QuizState.InDevelopment)
                return Failure(nameof(ErrorMessages.CantEditAsQuizNotInDevelopment));

            newTitle ??= string.Empty;
            dbQuiz.Title = newTitle;
            await DbContext.SaveChangesAsync();

            await QuizGroup(quizId).OnQuizTitleUpdatedAsync(quizId, newTitle);

            return Success();
        }

        [HubMethodName("OpenQuiz")]
        public async Task<HubResponse> OpenQuizAsync()
        {
            (Guid userId, Guid quizId, string? errorCode) = ExecutionContext;
            if (errorCode is not null)
                return Failure(errorCode);

            var dbQuiz = await DbContext.Quizzes.FindAsync(quizId);
            if (dbQuiz is null)
                return Failure(nameof(ErrorMessages.QuizNotFound));

            if (dbQuiz.OwnerId != userId)
                return Failure(nameof(ErrorMessages.QuizCantEditAsNotOwner));

            if (dbQuiz.State == QuizState.Open)
                return Failure(nameof(ErrorMessages.QuizAlreadyOpen));

            dbQuiz.State = QuizState.Open;
            dbQuiz.OpenedAt = DateTime.UtcNow;
            await DbContext.SaveChangesAsync();

            await QuizGroup(quizId).OnQuizOpenedAsync(quizId);

            return Success();
        }

        [HubMethodName("DeleteQuiz")]
        public async Task<HubResponse> DeleteQuizAsync()
        {
            (Guid userId, Guid quizId, string? errorCode) = ExecutionContext;
            if (errorCode is not null)
                return Failure(errorCode);

            var dbQuiz = await DbContext.Quizzes.FindAsync(quizId);
            if (dbQuiz is null)
                return Failure(nameof(ErrorMessages.QuizNotFound));

            if (dbQuiz.OwnerId != userId)
                return Failure(nameof(ErrorMessages.QuizCantEditAsNotOwner));

            if (dbQuiz.State != QuizState.InDevelopment)
                return Failure(nameof(ErrorMessages.QuizCantDeleteAsNotInDevelopment));

            DbContext.Quizzes.Remove(dbQuiz);
            await DbContext.SaveChangesAsync();

            await QuizGroup(quizId).OnQuizDeletedAsync(quizId);

            return Success();
        }
    }
}
