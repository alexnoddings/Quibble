using System;
using System.Collections.Generic;
using Quibble.Shared.Entities;

namespace Quibble.Server.Data.Models
{
    public class DbRound : IRound
    {
        public Guid Id { get; set; }
        public Guid QuizId { get; set; }
        public DbQuiz Quiz { get; set; } = default!;

        public string Title { get; set; } = string.Empty;
        public RoundState State { get; set; }
        public int Order { get; set; }

        public List<DbQuestion> Questions { get; set; } = new();
    }
}
