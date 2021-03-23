using System;

namespace Quibble.Shared.Models
{
    public class Round : IEntity
    {
        public Guid Id { get; set; }
        public Guid QuizId { get; set; }
        public string Title { get; set; } = string.Empty;
    }
}
