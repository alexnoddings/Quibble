using Quibble.Shared.Entities;

namespace Quibble.Client.Sync.Entities.HostMode
{
    public interface ISyncedHostModeQuestion : IQuestion, ISynchronisedEntity
    {
        public ISyncedHostModeRound Round { get; }
        public IReadOnlyList<ISyncedHostModeSubmittedAnswer> Answers { get; }

        public Task OpenAsync();
        public Task LockAsync();
        public Task ShowAnswer();
    }
}
