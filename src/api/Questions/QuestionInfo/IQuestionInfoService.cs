namespace Quibble.Questions.Info;

public interface IQuestionInfoService
{
    public ValueTask<QuestionInfo?> GetQuestionByIdAsync(Guid id);

    public ValueTask OnQuestionStateUpdatedAsync(Guid id);
    public ValueTask OnQuestionDeletedAsync(Guid id);
}
