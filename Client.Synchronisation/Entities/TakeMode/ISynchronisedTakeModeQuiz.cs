﻿namespace Quibble.Client.Sync.Entities.TakeMode
{
    public interface ISynchronisedTakeModeQuiz : ISynchronisedQuiz
    {
        public event Func<ISynchronisedTakeModeRound, Task>? RoundAdded;

        public IReadOnlyList<ISynchronisedTakeModeRound> Rounds { get; }
        public IReadOnlyList<ISynchronisedTakeModeParticipant> Participants { get; }
    }
}
