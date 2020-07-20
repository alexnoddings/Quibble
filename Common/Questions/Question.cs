using System;

namespace Quibble.Common.Questions
{
    public class Question : IEntity
    {
        public Guid Id { get; set; }
        public Guid RoundId { get; set; }

        public string Body { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public QuestionState State { get; set; }

        public Question()
        {
        }

        public Question(Question other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            Id = other.Id;
            RoundId = other.RoundId;

            Body = other.Body;
            Answer = other.Answer;
            State = other.State;
        }
    }
}
