namespace Quibble.Client.Sync.Core.Contexts;

public interface IQuizSyncContext
{
    public Task UpdateTitleAsync(string newTitle);
    public Task OpenAsync();
    public Task DeleteAsync();

    public event Func<string, Task> OnTitleUpdatedAsync;
    public event Func<Task> OnOpenedAsync;
    public event Func<Task> OnDeletedAsync;
}
