using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Client.Core.Contexts;
using Quibble.Sync.SignalR.Shared;

namespace Quibble.Sync.SignalR.Client.Contexts;

internal class SignalrSubmittedAnswerSyncContext : BaseSignalrSyncContext, ISubmittedAnswerSyncContext
{
	public event Func<Guid, string, Task>? OnTextUpdatedAsync;
	public event Func<Guid, decimal, Task>? OnAssignedPointsUpdatedAsync;

	public SignalrSubmittedAnswerSyncContext(ILogger<SignalrSubmittedAnswerSyncContext> logger, HubConnection hubConnection)
		: base(logger, hubConnection)
	{
		Bind(e => e.OnSubmittedAnswerTextUpdatedAsync, () => OnTextUpdatedAsync);
		Bind(e => e.OnSubmittedAnswerAssignedPointsUpdatedAsync, () => OnAssignedPointsUpdatedAsync);
	}

	public Task PreviewUpdateTextAsync(Guid id, string previewText) =>
		HubConnection.InvokeAsync(SignalrEndpoints.PreviewUpdateSubmittedAnswerText, id, previewText);

	public Task UpdateTextAsync(Guid id, string newText) =>
		HubConnection.InvokeAsync(SignalrEndpoints.UpdateSubmittedAnswerText, id, newText);

	public Task MarkAsync(Guid id, decimal points) =>
		HubConnection.InvokeAsync(SignalrEndpoints.UpdateSubmittedAnswerAssignedPoints, id, points);
}
