using System;
using System.Threading.Tasks;
using Quibble.Shared.Entities;

namespace Quibble.Client.Sync.Entities
{
    public interface ISynchronisedQuiz : ISynchronisedEntity, IQuiz, IAsyncDisposable
    {
        public event Func<Task>? Invalidated;
    }
}
