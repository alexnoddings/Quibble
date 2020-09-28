using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Quibble.Core.Entities;

namespace Quibble.Host.Common.Data.Entities
{
    public class DbParticipant : IParticipant
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        public DbQuibbleUser User { get; set; } = default!;

        [ForeignKey(nameof(Quiz))]
        public Guid QuizId { get; set; }

        public DbQuiz Quiz { get; set; } = default!;

        public List<DbParticipantAnswer> Answers { get; set; } = new List<DbParticipantAnswer>();
    }
}