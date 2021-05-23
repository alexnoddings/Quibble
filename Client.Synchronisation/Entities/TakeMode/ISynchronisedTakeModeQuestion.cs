using Quibble.Shared.Entities;

namespace Quibble.Client.Sync.Entities.TakeMode
{
    public interface ISynchronisedTakeModeQuestion : IQuestion, ISynchronisedEntity
    {
        public ISynchronisedTakeModeRound Round { get; }
        // May be null if the user joined after the question was locked
        public ISynchronisedTakeModeSubmittedAnswer? SubmittedAnswer { get; }
    }
}
