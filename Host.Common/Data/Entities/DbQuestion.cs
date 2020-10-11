using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Quibble.Core.Entities;

namespace Quibble.Host.Common.Data.Entities
{
    public class DbQuestion : IQuestion
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey(nameof(Round))]
        public Guid RoundId { get; set; }

        public DbRound Round { get; set; } = default!;

        public string QuestionText { get; set; } = string.Empty;

        public string CorrectAnswer { get; set; } = string.Empty;

        public QuestionState State { get; set; } = QuestionState.Hidden;

        public List<DbParticipantAnswer> Answers { get; set; } = new List<DbParticipantAnswer>();
    }
}