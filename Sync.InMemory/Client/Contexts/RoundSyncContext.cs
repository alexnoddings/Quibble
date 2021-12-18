using Microsoft.Extensions.Logging;
using Quibble.Client.Core.Contexts;
using Quibble.Client.Core.Extensions;
using Quibble.Common.Dtos;
using Quibble.Server.Core.Domain;

namespace Quibble.Sync.InMemory.Client.Contexts;

internal class RoundSyncContext : BaseSyncContext<RoundLogic>, IRoundSyncContext
{
	public event Func<RoundDto, Task>? OnAddedAsync;
	public event Func<Guid, string, Task>? OnTitleUpdatedAsync;
	public event Func<Guid, Task>? OnOpenedAsync;
	public event Func<Guid, int, Task>? OnOrderUpdatedAsync;
	public event Func<Guid, Task>? OnDeletedAsync;

	public RoundSyncContext(ILogger<RoundSyncContext> logger, SyncContext parent)
		: base(logger, parent)
	{
	}

	public Task AddAsync() =>
		InvokeAsync(rounds => rounds.CreateRoundAsync(Context));
	internal Task InvokeOnAddedAsync(RoundDto dto) =>
		OnAddedAsync.TryInvokeAsync(dto);

	public Task OpenAsync(Guid id) =>
		InvokeAsync(rounds => rounds.OpenRoundAsync(Context, id));
	internal Task InvokeOnOpenedAsync(Guid id) =>
		OnOpenedAsync.TryInvokeAsync(id);

	public Task UpdateTitleAsync(Guid id, string newTitle) =>
		InvokeAsync(rounds => rounds.UpdateRoundTitleAsync(Context, id, newTitle));
	internal Task InvokeOnTitleUpdatedAsync(Guid id, string newTitle) =>
		OnTitleUpdatedAsync.TryInvokeAsync(id, newTitle);

	internal Task InvokeOnOrderUpdatedAsync(Guid id, int newOrder) =>
		OnOrderUpdatedAsync.TryInvokeAsync(id, newOrder);

	public Task DeleteAsync(Guid id) =>
		InvokeAsync(rounds => rounds.DeleteRoundAsync(Context, id));
	internal Task InvokeOnDeletedAsync(Guid id) =>
		OnDeletedAsync.TryInvokeAsync(id);
}
