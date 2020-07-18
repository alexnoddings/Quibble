using System;

namespace Quibble.Common.SubmittedAnswers
{
    public class SubmittedAnswer : IEntity
    {
        public Guid Id { get; set; }
        public Guid ParticipantId { get; set; }
        public Guid QuestionId { get; set; }

        public string Value { get; set; } = string.Empty;
        public SubmittedAnswerStatus Status { get; set; }
    }
}
