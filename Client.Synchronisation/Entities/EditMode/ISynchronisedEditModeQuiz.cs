using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Quibble.Shared.Entities;
using Quibble.Shared.Models;

namespace Quibble.Client.Sync.Entities.EditMode
{
    public interface ISynchronisedEditModeQuiz : IQuiz, ISynchronisedEntity, IDisposable
    {
        public IEnumerable<ISynchronisedEditModeRound> Rounds { get; }

        public Task UpdateTitleAsync(string newTitle);
        public Task OpenAsync();
        public Task DeleteAsync();
        public Task AddRoundAsync();
    }
}
