﻿using System;
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
        [HubMethodName(Endpoints.CreateQuestion)]
        public async Task<HubResponse> CreateQuestionAsync(Guid roundId, string text, string answer, sbyte points)
        {
            (Guid userId, Guid quizId, string? errorCode) = ExecutionContext;
            if (errorCode is not null)
                return Failure(errorCode);

            var dbRound = await DbContext.Rounds.Include(round => round.Quiz).Include(round => round.Questions).FindAsync(roundId);
            if (dbRound is null)
                return Failure(nameof(ErrorMessages.QuestionParentRoundNotFound));

            if (dbRound.QuizId != quizId)
                return Failure(nameof(ErrorMessages.QuestionParentRoundNotFound));

            if (dbRound.Quiz.OwnerId != userId)
                return Failure(nameof(ErrorMessages.QuizCantEditAsNotOwner));

            if (dbRound.Quiz.State != QuizState.InDevelopment)
                return Failure(nameof(ErrorMessages.CantEditAsQuizNotInDevelopment));

            if (points < 1)
                return Failure(nameof(ErrorMessages.PointsCantBeLessThanOne));

            if (points > 10)
                return Failure(nameof(ErrorMessages.PointsCantBeMoreThanTen));

            var dbQuestion = new DbQuestion
            {
                Text = text ?? string.Empty,
                Answer = answer ?? string.Empty,
                Points = points,
                State = QuestionState.Hidden
            };
            dbRound.Questions.Add(dbQuestion);
            await DbContext.SaveChangesAsync();

            var question = Mapper.Map<QuestionDto>(dbQuestion);
            await QuizGroup(quizId).OnQuestionAddedAsync(question);

            return Success();
        }

        [HubMethodName(Endpoints.UpdateQuestionText)]
        public async Task<HubResponse> UpdateQuestionTextAsync(Guid questionId, string newText)
        {
            (Guid userId, Guid quizId, string? errorCode) = ExecutionContext;
            if (errorCode is not null)
                return Failure(errorCode);

            var dbQuestion = await DbContext.Questions.Include(question => question.Round).ThenInclude(round => round.Quiz).FindAsync(questionId);
            if (dbQuestion is null)
                return Failure(nameof(ErrorMessages.QuestionNotFound));

            if (dbQuestion.Round.QuizId != quizId)
                return Failure(nameof(ErrorMessages.QuestionNotFound));

            if (dbQuestion.Round.Quiz.OwnerId != userId)
                return Failure(nameof(ErrorMessages.QuizCantEditAsNotOwner));

            if (dbQuestion.Round.Quiz.State != QuizState.InDevelopment)
                return Failure(nameof(ErrorMessages.CantEditAsQuizNotInDevelopment));

            newText ??= string.Empty;
            dbQuestion.Text = newText;
            await DbContext.SaveChangesAsync();

            await QuizGroup(quizId).OnQuestionTextUpdatedAsync(dbQuestion.Id, newText);

            return Success();
        }

        [HubMethodName(Endpoints.UpdateQuestionAnswer)]
        public async Task<HubResponse> UpdateQuestionAnswerAsync(Guid questionId, string newAnswer)
        {
            (Guid userId, Guid quizId, string? errorCode) = ExecutionContext;
            if (errorCode is not null)
                return Failure(errorCode);

            var dbQuestion = await DbContext.Questions.Include(question => question.Round).ThenInclude(round => round.Quiz).FindAsync(questionId);
            if (dbQuestion is null)
                return Failure(nameof(ErrorMessages.QuestionNotFound));

            if (dbQuestion.Round.QuizId != quizId)
                return Failure(nameof(ErrorMessages.QuestionNotFound));

            if (dbQuestion.Round.Quiz.OwnerId != userId)
                return Failure(nameof(ErrorMessages.QuizCantEditAsNotOwner));

            if (dbQuestion.Round.Quiz.State != QuizState.InDevelopment)
                return Failure(nameof(ErrorMessages.CantEditAsQuizNotInDevelopment));

            newAnswer ??= string.Empty;
            dbQuestion.Answer = newAnswer;
            await DbContext.SaveChangesAsync();

            await QuizGroup(quizId).OnQuestionAnswerUpdatedAsync(dbQuestion.Id, newAnswer);

            return Success();
        }

        [HubMethodName(Endpoints.UpdateQuestionPoints)]
        public async Task<HubResponse> UpdateQuestionPointsAsync(Guid questionId, sbyte newPoints)
        {
            (Guid userId, Guid quizId, string? errorCode) = ExecutionContext;
            if (errorCode is not null)
                return Failure(errorCode);

            var dbQuestion = await DbContext.Questions.Include(question => question.Round).ThenInclude(round => round.Quiz).FindAsync(questionId);
            if (dbQuestion is null)
                return Failure(nameof(ErrorMessages.QuestionNotFound));

            if (dbQuestion.Round.QuizId != quizId)
                return Failure(nameof(ErrorMessages.QuestionNotFound));

            if (dbQuestion.Round.Quiz.OwnerId != userId)
                return Failure(nameof(ErrorMessages.QuizCantEditAsNotOwner));

            if (dbQuestion.Round.Quiz.State != QuizState.InDevelopment)
                return Failure(nameof(ErrorMessages.CantEditAsQuizNotInDevelopment));

            if (newPoints < 1)
                return Failure(nameof(ErrorMessages.PointsCantBeLessThanOne));

            if (newPoints > 10)
                return Failure(nameof(ErrorMessages.PointsCantBeMoreThanTen));

            dbQuestion.Points = newPoints;
            await DbContext.SaveChangesAsync();

            await QuizGroup(quizId).OnQuestionPointsUpdatedAsync(dbQuestion.Id, newPoints);

            return Success();
        }

        [HubMethodName(Endpoints.UpdateQuestionState)]
        public async Task<HubResponse> UpdateQuestionStateAsync(Guid questionId, QuestionState newState)
        {
            (Guid userId, Guid quizId, string? errorCode) = ExecutionContext;
            if (errorCode is not null)
                return Failure(errorCode);

            var dbQuestion = await DbContext.Questions.Include(question => question.Round).ThenInclude(round => round.Quiz).FindAsync(questionId);
            if (dbQuestion is null)
                return Failure(nameof(ErrorMessages.QuestionNotFound));

            if (dbQuestion.Round.QuizId != quizId)
                return Failure(nameof(ErrorMessages.QuestionNotFound));

            if (dbQuestion.Round.Quiz.OwnerId != userId)
                return Failure(nameof(ErrorMessages.QuizCantEditAsNotOwner));

            if (dbQuestion.Round.Quiz.State != QuizState.Open)
                return Failure(nameof(ErrorMessages.CantUpdateAsQuizNotOpen));

            // Can't update the state to be locked
            if (newState == QuestionState.Hidden)
                return Failure(nameof(ErrorMessages.QuestionBadState));

            switch (dbQuestion.State)
            {
                // The only valid state transitions
                case QuestionState.Hidden when newState == QuestionState.Open:
                case QuestionState.Open when newState == QuestionState.Locked:
                case QuestionState.Locked when newState == QuestionState.AnswerRevealed:
                    break;
                // Invalid new state value or invalid state transitions
                default:
                    return Failure(nameof(ErrorMessages.QuestionBadState));
            }

            dbQuestion.State = newState;
            await DbContext.SaveChangesAsync();

            await QuizGroup(quizId).OnQuestionStateUpdatedAsync(dbQuestion.Id, newState);

            return Success();
        }

        [HubMethodName(Endpoints.DeleteQuestion)]
        public async Task<HubResponse> DeleteQuestionAsync(Guid questionId)
        {
            (Guid userId, Guid quizId, string? errorCode) = ExecutionContext;
            if (errorCode is not null)
                return Failure(errorCode);

            var dbQuestion = await DbContext.Questions.Include(question => question.Round).ThenInclude(round => round.Quiz).FindAsync(questionId);
            if (dbQuestion is null)
                return Failure(nameof(ErrorMessages.QuestionNotFound));

            if (dbQuestion.Round.QuizId != quizId)
                return Failure(nameof(ErrorMessages.QuestionNotFound));

            if (dbQuestion.Round.Quiz.OwnerId != userId)
                return Failure(nameof(ErrorMessages.QuizCantEditAsNotOwner));

            if (dbQuestion.Round.Quiz.State != QuizState.InDevelopment)
                return Failure(nameof(ErrorMessages.CantEditAsQuizNotInDevelopment));

            DbContext.Questions.Remove(dbQuestion);
            await DbContext.SaveChangesAsync();

            await QuizGroup(quizId).OnQuestionDeletedAsync(dbQuestion.Id);

            return Success();
        }
    }
}