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
