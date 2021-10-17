using Quibble.Shared.Entities;

namespace Quibble.Client.Sync.Core
{
    public interface ISyncedEntity : IEntity
    {
        public event Func<Task>? Updated;

        public int GetStateStamp();
    }
}
