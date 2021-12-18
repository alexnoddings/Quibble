using Microsoft.Extensions.Logging;
using Quibble.Sync.InMemory.Server;

namespace Quibble.Sync.InMemory.Client.Contexts;

internal abstract class BaseSyncContext
{
	protected ILogger<BaseSyncContext> Logger { get; }
	protected SyncContext Parent { get; }
	protected SyncExecutionContext Context => Parent.SyncExecutionContext;

	protected BaseSyncContext(ILogger<BaseSyncContext> logger, SyncContext parent)
	{
		Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		Parent = parent ?? throw new ArgumentNullException(nameof(parent));
	}
}
