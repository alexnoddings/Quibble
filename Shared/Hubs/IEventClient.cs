using System;
using System.Threading.Tasks;
using Quibble.Shared.Models;

namespace Quibble.Shared.Hubs
{
    public interface IEventClient
    {
        public Task OnQuizTitleUpdatedAsync(Guid quizId, string newTitle);
        public Task OnQuizOpenedAsync(Guid quizId);
        public Task OnQuizDeletedAsync(Guid quizId);

        public Task OnRoundAddedAsync(Round round);
        public Task OnRoundTitleUpdatedAsync(Guid roundId, string newTitle);
        public Task OnRoundOpenedAsync(Guid roundId);
        public Task OnRoundDeletedAsync(Guid roundId);

        public Task OnQuestionAddedAsync(Question question);
        public Task OnQuestionTextUpdatedAsync(Guid questionId, string newText);
        public Task OnQuestionAnswerUpdatedAsync(Guid questionId, string newAnswer);
        public Task OnQuestionPointsUpdatedAsync(Guid questionId, sbyte newPoints);
        public Task OnQuestionStateUpdatedAsync(Guid questionId, QuestionState newState);
        public Task OnQuestionOpenedAsync(Guid questionId);
        public Task OnQuestionDeletedAsync(Guid questionId);

        public Task OnParticipantJoinedAsync(Guid id, string name);
    }
}
