namespace Quibble.Client.Sync.Entities.TakeMode
{
    public interface ISyncedTakeModeQuiz : ISynchronisedQuiz
    {
        public event Func<ISyncedTakeModeRound, Task>? RoundAdded;

        public IReadOnlyList<ISyncedTakeModeRound> Rounds { get; }
        public IReadOnlyList<ISyncedTakeModeParticipant> Participants { get; }
    }
}
