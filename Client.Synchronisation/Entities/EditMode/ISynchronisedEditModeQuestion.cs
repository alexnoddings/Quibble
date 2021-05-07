using System.Threading.Tasks;
using Quibble.Shared.Entities;

namespace Quibble.Client.Sync.Entities.EditMode
{
    public interface ISynchronisedEditModeQuestion : IQuestion, ISynchronisedEntity
    {
        public Task UpdateTextAsync(string newText);
        public Task UpdateAnswerAsync(string newAnswer);
        public Task UpdatePointsAsync(decimal newPoints);
        public Task UpdateStateAsync(QuestionState newState);
        public Task DeleteAsync();
    }
}
