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
    }
}
