using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Entities;

namespace Quibble.Client.Sync
{
    public abstract class BaseSynchronisedEntity : ISynchronisedEntity
    {
        protected ILogger<BaseSynchronisedEntity> Logger { get; }

        public virtual Guid Id { get; protected set; }

        public event Func<Task>? Updated;

        protected BaseSynchronisedEntity(ILogger<BaseSynchronisedEntity> logger)
        {
            Logger = logger;
        }

        protected Task OnUpdatedAsync() =>
            Updated?.Invoke() ?? Task.CompletedTask;

        public abstract int GetStateStamp();

        protected static int GenerateStateStamp(params object?[] stateProperties)
        {
            var hashCode = new HashCode();
            foreach (var property in stateProperties)
                if (property is not null)
                    BuildStateStamp(ref hashCode, property);

            return hashCode.ToHashCode();
        }

        private static void BuildStateStamp(ref HashCode hashCode, object obj)
        {
            if (obj is BaseSynchronisedEntity synchronisedEntity)
            {
                // Let synced entities add their own state stamp
                hashCode.Add(synchronisedEntity.GetStateStamp());
            }
            else if (obj is List<object> list)
            {
                // Check for list first as they're quicker to iterate than IEnumerables
                // Lists don't have their contents checked when added, need to add their items instead
                foreach (var listObj in list)
                {
                    BuildStateStamp(ref hashCode, listObj);
                }
            }
            else if (obj is IEnumerable<object> enumerable)
            {
                // Enumerables don't have their contents checked when added, need to add their items instead
                foreach (var enumerableObj in enumerable)
                {
                    BuildStateStamp(ref hashCode, enumerableObj);
                }
            }
            else
            {
                // If it isn't enumerable, just add it normally
                hashCode.Add(obj);
            }
        }
    }
}
