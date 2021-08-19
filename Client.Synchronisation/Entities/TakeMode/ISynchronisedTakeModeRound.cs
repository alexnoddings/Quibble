using Quibble.Shared.Entities;

namespace Quibble.Client.Sync.Entities.TakeMode
{
    public interface ISynchronisedTakeModeRound : IRound, ISynchronisedEntity
    {

        public event Func<ISynchronisedTakeModeQuestion, Task>? QuestionAdded;

        public ISynchronisedTakeModeQuiz Quiz { get; }
        public IReadOnlyList<ISynchronisedTakeModeQuestion> Questions { get; }
    }
}
