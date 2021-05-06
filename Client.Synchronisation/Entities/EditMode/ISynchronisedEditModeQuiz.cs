using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quibble.Client.Sync.Entities.EditMode
{
    public interface ISynchronisedEditModeQuiz : ISynchronisedQuiz
    {
        public IEnumerable<ISynchronisedEditModeRound> Rounds { get; }

        public Task UpdateTitleAsync(string newTitle);
        public Task OpenAsync();
        public Task DeleteAsync();
        public Task AddRoundAsync();
    }
}
