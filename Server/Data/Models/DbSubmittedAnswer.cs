using System;
using Quibble.Shared.Entities;

namespace Quibble.Server.Data.Models
{
    public class DbSubmittedAnswer : ISubmittedAnswer
    {
        public Guid Id { get; set; }
        public Guid QuestionId { get; set; }
        public DbQuestion Question { get; set; } = default!;
        public Guid ParticipantId { get; set; }
        public DbParticipant Participant { get; set; } = default!;

        public string Text { get; set; } = string.Empty;
        public sbyte AssignedPoints { get; set; }
    }
}
