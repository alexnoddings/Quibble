namespace Quibble.Client.Sync.Entities.EditMode
{
    public interface ISyncedEditModeQuiz : ISynchronisedQuiz
    {
        public event Func<Task>? OnInvalidated;

        public IReadOnlyList<ISyncedEditModeRound> Rounds { get; }

        public Task UpdateTitleAsync(string newTitle);
        public Task OpenAsync();
        public Task DeleteAsync();
        public Task AddRoundAsync();
    }
}
