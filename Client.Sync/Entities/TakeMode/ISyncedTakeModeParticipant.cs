using Quibble.Shared.Entities;

namespace Quibble.Client.Sync.Entities.TakeMode
{
    public interface ISyncedTakeModeParticipant : IParticipant, ISynchronisedEntity
    {
        public string UserName { get; }

        public ISyncedTakeModeQuiz Quiz { get; }
    }
}
