using System;
using System.Threading.Tasks;
using Quibble.Shared.Entities;
using Quibble.Shared.Models;

namespace Quibble.Shared.Hub
{
    public interface IQuibbleHubClient
    {
        public Task OnQuizTitleUpdatedAsync(string newTitle);
        public Task OnQuizOpenedAsync();
        public Task OnQuizDeletedAsync();

        public Task OnRoundAddedAsync(RoundDto round);
        public Task OnRoundTitleUpdatedAsync(Guid roundId, string newTitle);
        public Task OnRoundOpenedAsync(Guid roundId);
        public Task OnRoundDeletedAsync(Guid roundId);

        public Task OnQuestionAddedAsync(QuestionDto question);
        public Task OnQuestionTextUpdatedAsync(Guid questionId, string newText);
        public Task OnQuestionAnswerUpdatedAsync(Guid questionId, string newAnswer);
        public Task OnQuestionPointsUpdatedAsync(Guid questionId, sbyte newPoints);
        public Task OnQuestionStateUpdatedAsync(Guid questionId, QuestionState newState);
        public Task OnQuestionDeletedAsync(Guid questionId);

        public Task OnParticipantJoinedAsync(Guid id, string name);
    }
}
