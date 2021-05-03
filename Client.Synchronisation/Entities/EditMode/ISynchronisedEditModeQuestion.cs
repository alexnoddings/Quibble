using System.Threading.Tasks;
using Quibble.Shared.Entities;
using Quibble.Shared.Models;

namespace Quibble.Client.Sync.Entities.EditMode
{
    public interface ISynchronisedEditModeQuestion : IQuestion, ISynchronisedEntity
    {
        public Task UpdateTextAsync(string newText);
        public Task UpdateAnswerAsync(string newAnswer);
        public Task UpdatePointsAsync(sbyte newPoints);
        public Task UpdateStateAsync(QuestionState newState);
        public Task DeleteAsync();
    }
}
