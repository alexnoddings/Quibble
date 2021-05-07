using System.Collections.Generic;
using System.Threading.Tasks;
using Quibble.Shared.Entities;

namespace Quibble.Client.Sync.Entities.EditMode
{
    public interface ISynchronisedEditModeRound : IRound, ISynchronisedEntity
    {
        public IEnumerable<ISynchronisedEditModeQuestion> Questions { get; }

        public Task UpdateTitleAsync(string newTitle);
        public Task OpenAsync();
        public Task DeleteAsync();
        public Task AddQuestionAsync();
    }
}
