using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Client.Core.Contexts;
using Quibble.Common.Dtos;
using Quibble.Sync.SignalR.Shared;

namespace Quibble.Sync.SignalR.Client.Contexts;

internal class SignalrRoundSyncContext : BaseSignalrSyncContext, IRoundSyncContext
{
	public event Func<RoundDto, Task>? OnAddedAsync;
	public event Func<Guid, string, Task>? OnTitleUpdatedAsync;
	public event Func<Guid, Task>? OnOpenedAsync;
	public event Func<Guid, int, Task>? OnOrderUpdatedAsync;
	public event Func<Guid, Task>? OnDeletedAsync;

	public SignalrRoundSyncContext(ILogger<SignalrRoundSyncContext> logger, HubConnection hubConnection)
		: base(logger, hubConnection)
	{
		Bind(e => e.OnRoundAddedAsync, () => OnAddedAsync);
		Bind(e => e.OnRoundTitleUpdatedAsync, () => OnTitleUpdatedAsync);
		Bind(e => e.OnRoundOpenedAsync, () => OnOpenedAsync);
		Bind(e => e.OnRoundOrderUpdatedAsync, () => OnOrderUpdatedAsync);
		Bind(e => e.OnRoundDeletedAsync, () => OnDeletedAsync);
	}

	public Task AddAsync() =>
		HubConnection.InvokeAsync(SignalrEndpoints.CreateRound);

	public Task UpdateTitleAsync(Guid id, string newTitle) =>
		HubConnection.InvokeAsync(SignalrEndpoints.UpdateRoundTitle, id, newTitle);

	public Task OpenAsync(Guid id) =>
		HubConnection.InvokeAsync(SignalrEndpoints.OpenRound, id);

	public Task DeleteAsync(Guid id) =>
		HubConnection.InvokeAsync(SignalrEndpoints.DeleteRound, id);
}
