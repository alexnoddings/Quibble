using System;

namespace Quibble.Shared.Models
{
    public class SubmittedAnswer : IEntity
    {
        public Guid Id { get; set; }
        public Guid QuestionId { get; set; }
        public Guid ParticipantId { get; set; }
        public string Text { get; set; } = string.Empty;
        public sbyte AssignedPoints { get; set; } = -1;
    }
}
