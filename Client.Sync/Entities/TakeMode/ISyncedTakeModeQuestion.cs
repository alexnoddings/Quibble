using Quibble.Shared.Entities;

namespace Quibble.Client.Sync.Entities.TakeMode
{
    public interface ISyncedTakeModeQuestion : IQuestion, ISynchronisedEntity
    {
        public ISyncedTakeModeRound Round { get; }
        // May be null if the user joined after the question was locked
        public ISyncedTakeModeSubmittedAnswer? SubmittedAnswer { get; }
    }
}
