using Microsoft.Extensions.Logging;
using Quibble.Client.Core.Contexts;
using Quibble.Client.Core.Extensions;
using Quibble.Server.Core.Domain;

namespace Quibble.Sync.InMemory.Client.Contexts;

internal class SubmittedAnswerSyncContext : BaseSyncContext<SubmittedAnswerLogic>, ISubmittedAnswerSyncContext
{
	public event Func<Guid, string, Task>? OnTextUpdatedAsync;
	public event Func<Guid, decimal, Task>? OnAssignedPointsUpdatedAsync;

	public SubmittedAnswerSyncContext(ILogger<SubmittedAnswerSyncContext> logger, SyncContext parent)
		: base(logger, parent)
	{
	}

	public Task PreviewUpdateTextAsync(Guid id, string previewText) =>
		InvokeAsync(answers => answers.PreviewUpdateSubmittedAnswerTextAsync(Context, id, previewText));
	public Task UpdateTextAsync(Guid id, string newText) =>
		InvokeAsync(answers => answers.UpdateSubmittedAnswerTextAsync(Context, id, newText));
	internal Task InvokeOnTextUpdatedAsync(Guid id, string newText) =>
		OnTextUpdatedAsync.TryInvokeAsync(id, newText);

	public Task MarkAsync(Guid id, decimal points) =>
		InvokeAsync(answers => answers.UpdateSubmittedAnswerAssignedPointsAsync(Context, id, points));
	internal Task InvokeOnAssignedPointsUpdatedAsync(Guid id, decimal newPoints) =>
		OnAssignedPointsUpdatedAsync.TryInvokeAsync(id, newPoints);
}
