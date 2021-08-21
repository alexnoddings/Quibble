using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Quibble.Server.Data.Models;
using Quibble.Server.Extensions;
using Quibble.Shared.Entities;
using Quibble.Shared.Hub;
using Quibble.Shared.Models.Dtos;

namespace Quibble.Server.Hub
{
    public partial class QuibbleHub
    {
        [HubMethodName(Endpoints.CreateQuestion)]
        public async Task<HubResponse> CreateQuestionAsync(Guid roundId, string text, string answer, decimal points)
        {
            (Guid userId, Guid quizId, ApiError? error) = ExecutionContext;
            if (error is not null)
                return Failure(error);

            var dbRound =
                await DbContext.Rounds
                    .Include(round => round.Quiz)
                    .Include(round => round.Questions)
                    .FindAsync(roundId);

            if (dbRound is null)
                return Failure(HubErrors.QuestionParentRoundNotFound);

            if (dbRound.QuizId != quizId)
                return Failure(HubErrors.QuestionParentRoundNotFound);

            if (dbRound.Quiz.OwnerId != userId)
                return Failure(HubErrors.CantEditAsNotOwner);

            if (dbRound.Quiz.State != QuizState.InDevelopment)
                return Failure(HubErrors.CantDeleteAsNotInDevelopment);

            text ??= string.Empty;
            if (text.Length > 200)
                return Failure(HubErrors.TextTooLong);

            answer ??= string.Empty;
            if (answer.Length > 200)
                return Failure(HubErrors.TextTooLong);

            if (points < 0.25m)
                return Failure(HubErrors.PointsTooLow);

            if (points > 10m)
                return Failure(HubErrors.PointsTooHigh);

            // Ensure points are a division of 0.25
            points = Math.Round(points * 4, MidpointRounding.ToEven) / 4;

            var dbQuestion = new DbQuestion
            {
                Text = text,
                Answer = answer,
                Points = points,
                State = QuestionState.Hidden,
                Order = dbRound.Questions.Count
            };
            dbRound.Questions.Add(dbQuestion);
            await DbContext.SaveChangesAsync();

            var question = Mapper.Map<QuestionDto>(dbQuestion);
            await AllQuizUsersGroup(quizId).OnQuestionAddedAsync(question);

            return Success();
        }

        [HubMethodName(Endpoints.UpdateQuestionText)]
        public async Task<HubResponse> UpdateQuestionTextAsync(Guid questionId, string newText)
        {
            (Guid userId, Guid quizId, ApiError? error) = ExecutionContext;
            if (error is not null)
                return Failure(error);

            var dbQuestion =
                await DbContext.Questions
                    .Include(question => question.Round)
                        .ThenInclude(round => round.Quiz)
                    .FindAsync(questionId);

            if (dbQuestion is null)
                return Failure(HubErrors.QuestionNotFound);

            if (dbQuestion.Round.QuizId != quizId)
                return Failure(HubErrors.QuestionNotFound);

            if (dbQuestion.Round.Quiz.OwnerId != userId)
                return Failure(HubErrors.CantEditAsNotOwner);

            if (dbQuestion.Round.Quiz.State != QuizState.InDevelopment)
                return Failure(HubErrors.CantDeleteAsNotInDevelopment);

            newText ??= string.Empty;
            if (newText.Length > 200)
                return Failure(HubErrors.TextTooLong);

            dbQuestion.Text = newText;
            await DbContext.SaveChangesAsync();

            await AllQuizUsersGroup(quizId).OnQuestionTextUpdatedAsync(questionId, newText);

            return Success();
        }

        [HubMethodName(Endpoints.UpdateQuestionAnswer)]
        public async Task<HubResponse> UpdateQuestionAnswerAsync(Guid questionId, string newAnswer)
        {
            (Guid userId, Guid quizId, ApiError? error) = ExecutionContext;
            if (error is not null)
                return Failure(error);

            var dbQuestion =
                await DbContext.Questions
                    .Include(question => question.Round)
                        .ThenInclude(round => round.Quiz)
                    .FindAsync(questionId);

            if (dbQuestion is null)
                return Failure(HubErrors.QuestionNotFound);

            if (dbQuestion.Round.QuizId != quizId)
                return Failure(HubErrors.QuestionNotFound);

            if (dbQuestion.Round.Quiz.OwnerId != userId)
                return Failure(HubErrors.CantEditAsNotOwner);

            if (dbQuestion.Round.Quiz.State != QuizState.InDevelopment)
                return Failure(HubErrors.CantDeleteAsNotInDevelopment);

            newAnswer ??= string.Empty;
            if (newAnswer.Length > 200)
                return Failure(HubErrors.TextTooLong);

            dbQuestion.Answer = newAnswer;
            await DbContext.SaveChangesAsync();

            await AllQuizUsersGroup(quizId).OnQuestionAnswerUpdatedAsync(questionId, newAnswer);

            return Success();
        }

        [HubMethodName(Endpoints.UpdateQuestionPoints)]
        public async Task<HubResponse> UpdateQuestionPointsAsync(Guid questionId, decimal newPoints)
        {
            (Guid userId, Guid quizId, ApiError? error) = ExecutionContext;
            if (error is not null)
                return Failure(error);

            var dbQuestion =
                await DbContext.Questions
                    .Include(question => question.Round)
                        .ThenInclude(round => round.Quiz)
                    .FindAsync(questionId);

            if (dbQuestion is null)
                return Failure(HubErrors.QuestionNotFound);

            if (dbQuestion.Round.QuizId != quizId)
                return Failure(HubErrors.QuestionNotFound);

            if (dbQuestion.Round.Quiz.OwnerId != userId)
                return Failure(HubErrors.CantEditAsNotOwner);

            if (dbQuestion.Round.Quiz.State != QuizState.InDevelopment)
                return Failure(HubErrors.CantDeleteAsNotInDevelopment);

            if (newPoints < 0.25m)
                return Failure(HubErrors.PointsTooLow);

            if (newPoints > 10)
                return Failure(HubErrors.PointsTooHigh);

            // Ensure points are a division of 0.25
            newPoints = Math.Round(newPoints * 4, MidpointRounding.ToEven) / 4;

            dbQuestion.Points = newPoints;
            await DbContext.SaveChangesAsync();

            await AllQuizUsersGroup(quizId).OnQuestionPointsUpdatedAsync(questionId, newPoints);

            return Success();
        }

        [HubMethodName(Endpoints.UpdateQuestionState)]
        public async Task<HubResponse> UpdateQuestionStateAsync(Guid questionId, QuestionState newState)
        {
            (Guid userId, Guid quizId, ApiError? error) = ExecutionContext;
            if (error is not null)
                return Failure(error);

            var dbQuestion =
                await DbContext.Questions
                    .Include(question => question.Round)
                        .ThenInclude(round => round.Quiz)
                    .FindAsync(questionId);

            if (dbQuestion is null)
                return Failure(HubErrors.QuestionNotFound);

            if (dbQuestion.Round.QuizId != quizId)
                return Failure(HubErrors.QuestionNotFound);

            if (dbQuestion.Round.Quiz.OwnerId != userId)
                return Failure(HubErrors.CantEditAsNotOwner);

            if (dbQuestion.Round.Quiz.State != QuizState.Open)
                return Failure(HubErrors.CantUpdateAsNotOpen);

            if (dbQuestion.Round.State != RoundState.Open)
                return Failure(HubErrors.QuestionBadState);

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
                    return Failure(HubErrors.QuestionBadState);
            }

            if (newState == QuestionState.AnswerRevealed)
            {
                var areAnyAnswersUnmarked = await DbContext.SubmittedAnswers
                    .Where(submittedAnswer => submittedAnswer.QuestionId == dbQuestion.Id)
                    .AnyAsync(submittedAnswer => submittedAnswer.AssignedPoints == -1);

                if (areAnyAnswersUnmarked)
                    return Failure(HubErrors.QuestionBadState);
            }

            dbQuestion.State = newState;
            await DbContext.SaveChangesAsync();

            if (newState == QuestionState.Open)
            {
                await QuizHostGroup(quizId).OnQuestionStateUpdatedAsync(questionId, newState);

                var questionDto = Mapper.Map<QuestionDto>(dbQuestion);

                var submittedAnswers = await DbContext.SubmittedAnswers
                    .Where(submittedAnswer => submittedAnswer.QuestionId == dbQuestion.Id)
                    .ToListAsync();

                foreach (var submittedAnswer in submittedAnswers)
                {
                    var submittedAnswerDto = Mapper.Map<SubmittedAnswerDto>(submittedAnswer);
                    await QuizParticipantGroup(quizId, submittedAnswer.ParticipantId).OnQuestionRevealedAsync(questionDto, submittedAnswerDto);
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
                    await QuizHostGroup(quizId).OnSubmittedAnswerAssignedPointsUpdatedAsync(submittedAnswer.Id, 0);

                var answerLowerTrimmed = dbQuestion.Answer.Trim().ToLower();
                var correctSubmittedAnswers = await DbContext.SubmittedAnswers
                    .Where(submittedAnswer => submittedAnswer.QuestionId == dbQuestion.Id)
                    .Where(submittedAnswer => submittedAnswer.Text.Trim().ToLower() == answerLowerTrimmed)
                    .ToListAsync();


                foreach (var submittedAnswer in correctSubmittedAnswers)
                    submittedAnswer.AssignedPoints = dbQuestion.Points;

                await DbContext.SaveChangesAsync();

                foreach (var submittedAnswer in correctSubmittedAnswers)
                    await QuizHostGroup(quizId).OnSubmittedAnswerAssignedPointsUpdatedAsync(submittedAnswer.Id, submittedAnswer.AssignedPoints);
            }

            if (newState == QuestionState.AnswerRevealed)
            {
                var submittedAnswers = await DbContext.SubmittedAnswers
                    .Where(submittedAnswer => submittedAnswer.QuestionId == dbQuestion.Id)
                    .ToListAsync();

                foreach (var submittedAnswer in submittedAnswers)
                    await QuizParticipantGroup(quizId, submittedAnswer.ParticipantId).OnSubmittedAnswerAssignedPointsUpdatedAsync(submittedAnswer.Id, submittedAnswer.AssignedPoints);
            }

            await AllQuizUsersGroup(quizId).OnQuestionStateUpdatedAsync(questionId, newState);

            return Success();
        }

        [HubMethodName(Endpoints.DeleteQuestion)]
        public async Task<HubResponse> DeleteQuestionAsync(Guid questionId)
        {
            (Guid userId, Guid quizId, ApiError? error) = ExecutionContext;
            if (error is not null)
                return Failure(error);

            var dbQuestion =
                await DbContext.Questions
                    .Include(question => question.Round)
                        .ThenInclude(round => round.Quiz)
                    .Include(question => question.Round)
                        .ThenInclude(round => round.Questions)
                    .FindAsync(questionId);

            if (dbQuestion is null)
                return Failure(HubErrors.QuestionNotFound);

            if (dbQuestion.Round.QuizId != quizId)
                return Failure(HubErrors.QuestionNotFound);

            if (dbQuestion.Round.Quiz.OwnerId != userId)
                return Failure(HubErrors.CantEditAsNotOwner);

            if (dbQuestion.Round.Quiz.State != QuizState.InDevelopment)
                return Failure(HubErrors.CantDeleteAsNotInDevelopment);

            DbContext.Questions.Remove(dbQuestion);
            dbQuestion.Round.Questions.Remove(dbQuestion);

            List<DbQuestion> modifiedQuestions = new(dbQuestion.Round.Questions.Count);
            int questionCount = 0;
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

            var allQuizUsersGroup = AllQuizUsersGroup(quizId);
            await allQuizUsersGroup.OnQuestionDeletedAsync(dbQuestion.Id);
            foreach (var modifiedQuestion in modifiedQuestions)
                await allQuizUsersGroup.OnQuestionOrderUpdatedAsync(modifiedQuestion.Id, modifiedQuestion.Order);

            return Success();
        }
    }
}
