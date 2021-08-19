using Quibble.Shared.Entities;

namespace Quibble.Client.Sync.Entities.HostMode
{
    public interface ISynchronisedHostModeQuestion : IQuestion, ISynchronisedEntity
    {
        public ISynchronisedHostModeRound Round { get; }
        public IReadOnlyList<ISynchronisedHostModeSubmittedAnswer> Answers { get; }

        public Task OpenAsync();
        public Task LockAsync();
        public Task ShowAnswer();
    }
}
