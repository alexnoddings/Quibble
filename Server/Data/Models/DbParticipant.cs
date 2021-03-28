using System.Collections.Generic;
using Quibble.Shared.Models;

namespace Quibble.Server.Data.Models
{
    public class DbParticipant : Participant
    {
        public DbQuiz Quiz { get; set; } = default!;
        public AppUser User { get; set; } = default!;
        public List<DbSubmittedAnswer> SubmittedAnswers { get; set; } = new();
    }
}
