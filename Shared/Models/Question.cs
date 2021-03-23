using System;

namespace Quibble.Shared.Models
{
    public class Question : IEntity
    {
        public Guid Id { get; set; }
        public Guid RoundId { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
    }
}
