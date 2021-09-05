using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Contexts;
using Quibble.Client.Sync.Core;

namespace Quibble.Client.Sync
{
    public abstract class SyncedEntity : ISyncedEntity
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

        protected Task OnUpdatedAsync() =>
            Updated?.Invoke() ?? Task.CompletedTask;

        public abstract int GetStateStamp();
    }
}
