using System;
using System.Threading.Tasks;

namespace Quibble.Client.Sync.Entities
{
    public interface ISynchronisedEntity
    {
        public event Func<Task>? Updated;

        public int GetStateStamp();
    }
}
