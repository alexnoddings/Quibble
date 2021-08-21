using Quibble.Shared.Entities;

namespace Quibble.Client.Sync.Entities.TakeMode
{
    public interface ISyncedTakeModeRound : IRound, ISynchronisedEntity
    {

        public event Func<ISyncedTakeModeQuestion, Task>? QuestionAdded;

        public ISyncedTakeModeQuiz Quiz { get; }
        public IReadOnlyList<ISyncedTakeModeQuestion> Questions { get; }
    }
}
