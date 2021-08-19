using Quibble.Shared.Entities;

namespace Quibble.Client.Sync.Entities
{
    public interface ISynchronisedEntity : IEntity
    {
        public event Func<Task>? Updated;

        public int GetStateStamp();
    }
}
