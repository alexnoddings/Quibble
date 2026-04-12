namespace Quibble.Questions.Info;

public interface IAnswerInfoService
{
    public ValueTask<AnswerInfo?> GetAnswerToQuestionByParticipantAsync(Guid questionId, Guid participantId);
    public ValueTask<AnswerInfo?> GetAnswerToQuestionByUserAsync(Guid questionId, Guid userId);

    public ValueTask OnQuestionStateUpdatedAsync(Guid participantId, Guid questionId);
    public ValueTask OnQuestionDeletedAsync(Guid participantId, Guid questionId);
}
