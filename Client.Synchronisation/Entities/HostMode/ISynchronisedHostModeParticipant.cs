using Quibble.Shared.Entities;

namespace Quibble.Client.Sync.Entities.HostMode
{
    public interface ISynchronisedHostModeParticipant : IParticipant, ISynchronisedEntity
    {
        public string UserName { get; }

        public ISynchronisedHostModeQuiz Quiz { get; }
        public IReadOnlyList<ISynchronisedHostModeSubmittedAnswer> Answers { get; }
    }
}
