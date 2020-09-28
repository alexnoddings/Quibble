using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Quibble.Core.Entities;

namespace Quibble.Host.Common.Data.Entities
{
    public class DbQuiz : IQuiz
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey(nameof(Owner))]
        public Guid OwnerId { get; set; }

        public DbQuibbleUser Owner { get; set; } = default!;

        public string Title { get; set; } = string.Empty;

        public DateTime? PublishedAt { get; set; }

        public bool IsPublished => PublishedAt != null;

        public List<DbRound> Rounds { get; set; } = new List<DbRound>();

        public List<DbParticipant> Participants { get; set; } = new List<DbParticipant>();
    }
}