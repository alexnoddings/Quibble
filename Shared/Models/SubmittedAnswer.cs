using System;

namespace Quibble.Shared.Models
{
    public class SubmittedAnswer : IEntity
    {
        public Guid Id { get; set; }
        public Guid QuestionId { get; set; }
        public Guid SubmitterId { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
