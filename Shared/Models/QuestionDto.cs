using System;
using Quibble.Shared.Entities;

namespace Quibble.Shared.Models
{
    public class QuestionDto : IQuestion
    {
        public Guid Id { get; set; }
        public Guid RoundId { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public sbyte Points { get; set; }
        public QuestionState State { get; set; }
    }
}
