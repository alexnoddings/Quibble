namespace Quibble.Client.Sync.Core.Contexts;

public interface ISubmittedAnswerSyncContext
{
    public Task PreviewUpdateTextAsync(Guid id, string previewText);
    public Task UpdateTextAsync(Guid id, string newText);
    public Task MarkAsync(Guid id, decimal points);

    public event Func<Guid, string, Task> OnTextUpdatedAsync;
    public event Func<Guid, decimal, Task> OnAssignedPointsUpdatedAsync;
}
