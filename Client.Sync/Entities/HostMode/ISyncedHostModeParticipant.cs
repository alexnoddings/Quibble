using Quibble.Shared.Entities;

namespace Quibble.Client.Sync.Entities.HostMode
{
    public interface ISyncedHostModeParticipant : IParticipant, ISynchronisedEntity
    {
        public string UserName { get; }

        public ISyncedHostModeQuiz Quiz { get; }
        public IReadOnlyList<ISyncedHostModeSubmittedAnswer> Answers { get; }
    }
}
