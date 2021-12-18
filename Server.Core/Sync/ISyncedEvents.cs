using Quibble.Common.Dtos;
using Quibble.Common.Entities;

namespace Quibble.Server.Core.Sync;

public interface ISyncedEvents
{
	public Task OnQuizTitleUpdatedAsync(string newTitle);
	public Task OnQuizOpenedAsync();
	public Task OnQuizDeletedAsync();

	public Task OnParticipantJoinedAsync(ParticipantDto participant, List<SubmittedAnswerDto> submittedAnswers);

	public Task OnRoundAddedAsync(RoundDto round);
	public Task OnRoundTitleUpdatedAsync(Guid roundId, string newTitle);
	public Task OnRoundOrderUpdatedAsync(Guid roundId, int newOrder);
	public Task OnRoundOpenedAsync(Guid roundId);
	public Task OnRoundDeletedAsync(Guid roundId);

	public Task OnQuestionAddedAsync(QuestionDto question);
	public Task OnQuestionTextUpdatedAsync(Guid questionId, string newText);
	public Task OnQuestionAnswerUpdatedAsync(Guid questionId, string newAnswer);
	public Task OnQuestionPointsUpdatedAsync(Guid questionId, decimal newPoints);
	public Task OnQuestionStateUpdatedAsync(Guid questionId, QuestionState newState);
	public Task OnQuestionOrderUpdatedAsync(Guid questionId, int newOrder);
	public Task OnQuestionDeletedAsync(Guid questionId);
	public Task OnQuestionRevealedAsync(QuestionDto question, List<SubmittedAnswerDto> submittedAnswers);

	public Task OnSubmittedAnswerTextUpdatedAsync(Guid submittedAnswerId, string newText);
	public Task OnSubmittedAnswerAssignedPointsUpdatedAsync(Guid submittedAnswerId, decimal newPoints);
}
