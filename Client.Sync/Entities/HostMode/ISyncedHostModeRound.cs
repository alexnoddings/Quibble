using Quibble.Shared.Entities;

namespace Quibble.Client.Sync.Entities.HostMode
{
    public interface ISyncedHostModeRound : IRound, ISynchronisedEntity
    {
        public ISyncedHostModeQuiz Quiz { get; }
        public IReadOnlyList<ISyncedHostModeQuestion> Questions { get; }

        public Task OpenAsync();
    }
}
