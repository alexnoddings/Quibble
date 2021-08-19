using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Quibble.Server.Extensions;
using Quibble.Shared.Entities;
using Quibble.Shared.Hub;

namespace Quibble.Server.Hub
{
    public partial class QuibbleHub
    {
        private async Task<HubResponse> UpdateSubmittedAnswerTextCoreAsync(Guid answerId, string newText, bool shouldPersist)
        {
            (Guid userId, Guid quizId, ApiError? error) = ExecutionContext;
            if (error is not null)
                return Failure(error);

            var dbAnswer =
                await DbContext.SubmittedAnswers
                    .Include(answer => answer.Participant)
                    .Include(answer => answer.Question)
                        .ThenInclude(question => question.Round)
                            .ThenInclude(round => round.Quiz)
                    .FindAsync(answerId);

            if (dbAnswer is null)
                return Failure(HubErrors.AnswerNotFound);

            if (dbAnswer.Question.Round.QuizId != quizId)
                return Failure(HubErrors.QuestionParentRoundNotFound);

            if (dbAnswer.Participant.UserId != userId)
                return Failure(HubErrors.CantEditAsNotOwner);

            if (dbAnswer.Question.State != QuestionState.Open)
                return Failure(HubErrors.QuestionBadState);

            newText ??= string.Empty;
            if (newText.Length > 200)
                return Failure(HubErrors.TextTooLong);

            if (shouldPersist)
            {
                dbAnswer.Text = newText;
                await DbContext.SaveChangesAsync();
            }
            await QuizHostGroup(quizId).OnSubmittedAnswerTextUpdatedAsync(answerId, newText);
            var quizParticipantExceptConnection = Clients.OthersInGroup(GetQuizParticipantGroupName(quizId, dbAnswer.ParticipantId));
            await quizParticipantExceptConnection.OnSubmittedAnswerTextUpdatedAsync(answerId, newText);

            return Success();
        }

        [HubMethodName(Endpoints.PreviewUpdateSubmittedAnswerText)]
        public Task<HubResponse> PreviewUpdateSubmittedAnswerTextAsync(Guid answerId, string newText) =>
            UpdateSubmittedAnswerTextCoreAsync(answerId, newText, false);

        [HubMethodName(Endpoints.UpdateSubmittedAnswerText)]
        public Task<HubResponse> UpdateSubmittedAnswerTextAsync(Guid answerId, string newText) =>
            UpdateSubmittedAnswerTextCoreAsync(answerId, newText, true);

        [HubMethodName(Endpoints.UpdateSubmittedAnswerAssignedPoints)]
        public async Task<HubResponse> UpdateSubmittedAnswerAssignedPointsAsync(Guid answerId, decimal points)
        {
            (Guid userId, Guid quizId, ApiError? error) = ExecutionContext;
            if (error is not null)
                return Failure(error);

            var dbAnswer =
                await DbContext.SubmittedAnswers
                    .Include(answer => answer.Participant)
                    .Include(answer => answer.Question)
                        .ThenInclude(question => question.Round)
                            .ThenInclude(round => round.Quiz)
                    .FindAsync(answerId);

            if (dbAnswer is null)
                return Failure(HubErrors.AnswerNotFound);

            if (dbAnswer.Question.Round.QuizId != quizId)
                return Failure(HubErrors.QuestionParentRoundNotFound);

            if (dbAnswer.Question.Round.Quiz.OwnerId != userId)
                return Failure(HubErrors.CantEditAsNotOwner);

            if (dbAnswer.Question.State < QuestionState.Locked)
                return Failure(HubErrors.QuestionBadState);

            if (points < 0)
                return Failure(HubErrors.PointsTooLow);

            if (points > 10m)
                return Failure(HubErrors.PointsTooHigh);

            // Ensure points are a division of 0.25
            points = Math.Round(points * 4, MidpointRounding.ToEven) / 4;
            dbAnswer.AssignedPoints = points;
            await DbContext.SaveChangesAsync();

            await QuizHostGroup(quizId).OnSubmittedAnswerAssignedPointsUpdatedAsync(answerId, points);
            if (dbAnswer.Question.State == QuestionState.AnswerRevealed)
                await QuizParticipantGroup(quizId, dbAnswer.ParticipantId).OnSubmittedAnswerAssignedPointsUpdatedAsync(answerId, points);

            return Success();
        }
    }
}
