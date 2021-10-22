using Quibble.Shared.Entities;

namespace Quibble.Client.Sync.Core.Entities;

public interface ISyncedRound : ISyncedEntity, IRound
{
    public ISyncedQuiz Quiz { get; }
    public ISyncedEntities<ISyncedQuestion> Questions { get; }

    public Task AddQuestionAsync();
    public Task UpdateTitleAsync(string newTitle);
    public Task OpenAsync();
    public Task DeleteAsync();
}
