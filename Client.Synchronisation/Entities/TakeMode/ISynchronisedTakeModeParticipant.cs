using Quibble.Shared.Entities;

namespace Quibble.Client.Sync.Entities.TakeMode
{
    public interface ISynchronisedTakeModeParticipant : IParticipant, ISynchronisedEntity
    {
        public string UserName { get; }

        public ISynchronisedTakeModeQuiz Quiz { get; }
    }
}
