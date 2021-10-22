using Quibble.Shared.Models.Dtos;

namespace Quibble.Client.Sync.Core.Contexts;

public interface IRoundSyncContext
{
    public Task AddAsync();
    public Task UpdateTitleAsync(Guid id, string newTitle);
    public Task OpenAsync(Guid id);
    public Task DeleteAsync(Guid id);

    public event Func<RoundDto, Task> OnAddedAsync;
    public event Func<Guid, string, Task> OnTitleUpdatedAsync;
    public event Func<Guid, Task> OnOpenedAsync;
    public event Func<Guid, int, Task> OnOrderUpdatedAsync;
    public event Func<Guid, Task> OnDeletedAsync;
}
