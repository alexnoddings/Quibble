using System;

namespace Quibble.Common.Quizzes
{
    public class Quiz : IEntity
    {
        public Guid Id { get; set; }

        public string OwnerId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public QuizState State { get; set; }

        public Quiz()
        {
        }

        public Quiz(Quiz other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            Id = other.Id;
            OwnerId = other.OwnerId;
            Title = other.Title;
            State = other.State;
        }
    }
}
