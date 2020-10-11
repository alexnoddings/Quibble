using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Quibble.Core.Entities;

namespace Quibble.Host.Common.Data.Entities
{
    public class DbRound : IRound
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey(nameof(Quiz))]
        public Guid QuizId { get; set; }

        public DbQuiz Quiz { get; set; } = default!;

        public string Title { get; set; } = string.Empty;

        public RoundState State { get; set; } = RoundState.Hidden;

        public List<DbQuestion> Questions { get; set; } = new List<DbQuestion>();
    }
}