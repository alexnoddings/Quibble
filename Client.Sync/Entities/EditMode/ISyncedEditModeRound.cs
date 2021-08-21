using Quibble.Shared.Entities;

namespace Quibble.Client.Sync.Entities.EditMode
{
    public interface ISyncedEditModeRound : IRound, ISynchronisedEntity
    {
        public ISyncedEditModeQuiz Quiz { get; }
        public IReadOnlyList<ISyncedEditModeQuestion> Questions { get; }

        public Task UpdateTitleAsync(string newTitle);
        public Task DeleteAsync();
        public Task AddQuestionAsync();
    }
}
