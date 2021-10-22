using Quibble.Shared.Entities;

namespace Quibble.Client.Sync.Core.Entities;

public interface ISyncedQuestion : ISyncedEntity, IQuestion
{
    public ISyncedRound Round { get; }
    public ISyncedEntities<ISyncedSubmittedAnswer> SubmittedAnswers { get; }

    public Task UpdateTextAsync(string newText);
    public Task UpdateAnswerAsync(string newAnswer);
    public Task UpdatePointsAsync(decimal newPoints);
    public Task UpdateStateAsync(QuestionState newState);
    public Task DeleteAsync();
}
