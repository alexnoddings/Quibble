using Microsoft.Extensions.Logging;
using Quibble.Client.Core.Contexts;
using System.Runtime.CompilerServices;

namespace Quibble.Client.Core.Entities.Synced;

internal abstract class SyncedEntity : ISyncedEntity
{
	protected ILogger<SyncedEntity> Logger { get; }
	protected ISyncContext SyncContext { get; }

	public virtual Guid Id { get; protected set; }

	public event Func<Task>? Updated;

	protected SyncedEntity(ILogger<SyncedEntity> logger, ISyncContext syncContext)
	{
		Logger = logger;
		SyncContext = syncContext;
	}

	protected Task OnUpdatedAsync([CallerMemberName] string callerName = "")
	{
		if (callerName != "")
			Logger.LogTrace("{UpdateInvocationMethod} invoked on {Type}.", callerName, GetType().Name);

		return Updated?.Invoke() ?? Task.CompletedTask;
	}

	public abstract int GetStateStamp();
}
