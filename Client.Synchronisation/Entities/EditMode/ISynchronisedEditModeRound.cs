using Quibble.Shared.Entities;

namespace Quibble.Client.Sync.Entities.EditMode
{
    public interface ISynchronisedEditModeRound : IRound, ISynchronisedEntity
    {
        public ISynchronisedEditModeQuiz Quiz { get; }
        public IReadOnlyList<ISynchronisedEditModeQuestion> Questions { get; }

        public Task UpdateTitleAsync(string newTitle);
        public Task DeleteAsync();
        public Task AddQuestionAsync();
    }
}
