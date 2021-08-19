namespace Quibble.Client.Sync.Entities.HostMode
{
    public interface ISynchronisedHostModeQuiz : ISynchronisedQuiz
    {
        public IReadOnlyList<ISynchronisedHostModeRound> Rounds { get; }
        public IReadOnlyList<ISynchronisedHostModeParticipant> Participants { get; }
    }
}
