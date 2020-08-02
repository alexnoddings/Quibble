using System;

namespace Quibble.Common.Rounds
{
    public class Round : IEntity
    {
        public Guid Id { get; set; }
        public Guid QuizId { get; set; }

        public string Title { get; set; } = string.Empty;
        public RoundState State { get; set; }

        public Round()
        {
        }

        public Round(Round other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            Id = other.Id;
            QuizId = other.QuizId;

            Title = other.Title;
            State = other.State;
        }
    }
}
