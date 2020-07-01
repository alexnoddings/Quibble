using System;

namespace Quibble.Server.Models.Questions
{
    public class Question : IEntity<Guid>
    {
        public Guid Id { get; }
        public Guid RoundId { get; set; }

        public string Body { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public QuestionState State { get; set; } = QuestionState.Hidden;
    }
}
