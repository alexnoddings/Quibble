using System.Collections.Generic;
using System.Threading.Tasks;
using Quibble.Shared.Entities;

namespace Quibble.Client.Sync.Entities.HostMode
{
    public interface ISynchronisedHostModeRound : IRound, ISynchronisedEntity
    {
        public ISynchronisedHostModeQuiz Quiz { get; }
        public IReadOnlyList<ISynchronisedHostModeQuestion> Questions { get; }

        public Task OpenAsync();
    }
}
