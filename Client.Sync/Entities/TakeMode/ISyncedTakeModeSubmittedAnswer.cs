using Quibble.Shared.Entities;

namespace Quibble.Client.Sync.Entities.TakeMode
{
    public interface ISyncedTakeModeSubmittedAnswer : ISubmittedAnswer, ISynchronisedEntity
    {
        public ISyncedTakeModeQuestion Question { get; }

        public Task PreviewUpdateTextAsync(string previewText);
        public Task UpdateTextAsync(string newText);
    }
}
