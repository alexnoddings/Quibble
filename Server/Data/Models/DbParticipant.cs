using System;
using System.Collections.Generic;
using Quibble.Shared.Entities;

namespace Quibble.Server.Data.Models
{
    public class DbParticipant : IParticipant
    {
        public Guid Id { get; set; }
        public Guid QuizId { get; set; }
        public DbQuiz Quiz { get; set; } = default!;
        public Guid UserId { get; set; }
        public AppUser User { get; set; } = default!;

        public List<DbSubmittedAnswer> SubmittedAnswers { get; set; } = new();
    }
}
