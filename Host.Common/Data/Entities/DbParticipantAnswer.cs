using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Quibble.Core.Entities;

namespace Quibble.Host.Common.Data.Entities
{
    public class DbParticipantAnswer : IParticipantAnswer
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey(nameof(Question))]
        public Guid QuestionId { get; set; }

        public DbQuestion Question { get; set; } = default!;

        [ForeignKey(nameof(Participant))]
        public Guid ParticipantId { get; set; }

        public DbParticipant Participant { get; set; } = default!;

        public string Text { get; set; } = string.Empty;

        public AnswerMark Mark { get; set; } = AnswerMark.Unmarked;
    }
}