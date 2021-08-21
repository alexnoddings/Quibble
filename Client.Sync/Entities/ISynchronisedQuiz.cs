using Quibble.Shared.Entities;

namespace Quibble.Client.Sync.Entities
{
    public interface ISynchronisedQuiz : ISynchronisedEntity, IQuiz, IAsyncDisposable
    {
    }
}
