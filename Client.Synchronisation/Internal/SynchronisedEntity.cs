using System;
using System.Threading.Tasks;
using Quibble.Client.Sync.Entities;

namespace Quibble.Client.Sync.Internal
{
    public abstract class SynchronisedEntity : ISynchronisedEntity
    {
        public event Func<Task>? Updated;

        protected Task OnUpdatedAsync() =>
            Updated?.Invoke() is null
                ? Task.CompletedTask
                : Updated.Invoke();
    }
}
