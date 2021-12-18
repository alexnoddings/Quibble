using Quibble.Sync.InMemory.Client.Contexts;

namespace Quibble.Sync.InMemory.Server;

internal class SyncContextStore : List<SyncContext>
{
	public void TryAdd(SyncContext context)
	{
		if (this.Any(ctx => ctx.InstanceId == context.InstanceId))
			return;
		Add(context);
	}
}
