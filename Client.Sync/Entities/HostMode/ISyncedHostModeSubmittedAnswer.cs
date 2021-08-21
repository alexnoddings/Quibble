using Quibble.Shared.Entities;

namespace Quibble.Client.Sync.Entities.HostMode
{
    public interface ISyncedHostModeSubmittedAnswer : ISubmittedAnswer, ISynchronisedEntity
    {
        public ISyncedHostModeQuestion Question { get; }
        public ISyncedHostModeParticipant Submitter { get; }

        public Task MarkAsync(decimal points);
    }
}
