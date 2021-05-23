using System.Threading.Tasks;
using Quibble.Shared.Entities;

namespace Quibble.Client.Sync.Entities.TakeMode
{
    public interface ISynchronisedTakeModeSubmittedAnswer : ISubmittedAnswer, ISynchronisedEntity
    {
        public ISynchronisedTakeModeQuestion Question { get; }

        public Task PreviewUpdateTextAsync(string previewText);
        public Task UpdateTextAsync(string newText);
    }
}
