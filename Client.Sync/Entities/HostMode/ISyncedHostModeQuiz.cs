namespace Quibble.Client.Sync.Entities.HostMode
{
    public interface ISyncedHostModeQuiz : ISynchronisedQuiz
    {
        public IReadOnlyList<ISyncedHostModeRound> Rounds { get; }
        public IReadOnlyList<ISyncedHostModeParticipant> Participants { get; }
    }
}
