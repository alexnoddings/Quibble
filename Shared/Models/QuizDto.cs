using System;
using Quibble.Shared.Entities;

namespace Quibble.Shared.Models
{
    public class QuizDto : IQuiz
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public string Title { get; set; } = string.Empty;
        public QuizState State { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? OpenedAt { get; set; }
    }
}
