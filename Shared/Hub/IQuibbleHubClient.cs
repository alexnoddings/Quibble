using Quibble.Shared.Entities;
using Quibble.Shared.Models.Dtos;

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
        public Task OnQuestionPointsUpdatedAsync(Guid questionId, decimal newPoints);
        public Task OnQuestionStateUpdatedAsync(Guid questionId, QuestionState newState);
        public Task OnQuestionDeletedAsync(Guid questionId);
        public Task OnQuestionRevealedAsync(QuestionDto question, SubmittedAnswerDto submittedAnswer);

        public Task OnParticipantJoinedAsync(ParticipantDto participant, List<SubmittedAnswerDto> submittedAnswers);

        public Task OnSubmittedAnswerTextUpdatedAsync(Guid submittedAnswerId, string newText);
        public Task OnSubmittedAnswerAssignedPointsUpdatedAsync(Guid submittedAnswerId, decimal newPoints);
    }
}
