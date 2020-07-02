using System;

namespace Quibble.Server.Models.Rounds
{
    public class Round : IEntity<Guid>
    {
        public Guid Id { get; }
        public Guid QuizId { get; }

        public string Title { get; set; } = "New Round";
        public RoundState State { get; set; } = RoundState.Hidden;
    }
}
