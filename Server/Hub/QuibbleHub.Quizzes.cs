using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Quibble.Server.Extensions;
using Quibble.Shared.Entities;
using Quibble.Shared.Hub;
using Quibble.Shared.Models;
using Quibble.Shared.Resources;

namespace Quibble.Server.Hub
{
    public partial class QuibbleHub
    {
        [HubMethodName(Endpoints.GetQuiz)]
        public async Task<HubResponse<FullQuizDto>> GetQuizAsync()
        {
            (Guid userId, Guid quizId, string? errorCode) = ExecutionContext;
            if (errorCode is not null)
                return Failure<FullQuizDto>(errorCode);

            var dbQuiz = await DbContext.Quizzes.Include(q => q.Rounds).ThenInclude(r => r.Questions).FindAsync(quizId);
            if (dbQuiz is null)
                return Failure<FullQuizDto>(nameof(ErrorMessages.QuizNotFound));

            if (dbQuiz.OwnerId != userId && dbQuiz.State != QuizState.Open)
                return Failure<FullQuizDto>(nameof(ErrorMessages.QuizNotOpen));

            var quiz = Mapper.Map<QuizDto>(dbQuiz);
            var rounds = Mapper.Map<List<RoundDto>>(dbQuiz.Rounds);
            var questions = Mapper.Map<List<QuestionDto>>(dbQuiz.Rounds.SelectMany(r => r.Questions));

            return Success(new FullQuizDto(quiz, rounds, questions));
        }

        [HubMethodName(Endpoints.UpdateQuizTitle)]
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

            await QuizGroupExceptCurrent(quizId).OnQuizTitleUpdatedAsync(newTitle);

            return Success();
        }

        [HubMethodName(Endpoints.OpenQuiz)]
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

            await QuizGroup(quizId).OnQuizOpenedAsync();

            return Success();
        }

        [HubMethodName(Endpoints.DeleteQuiz)]
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

            await QuizGroup(quizId).OnQuizDeletedAsync();

            return Success();
        }
    }
}
