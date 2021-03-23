using System.Collections.Generic;
using Quibble.Shared.Models;

namespace Quibble.Server.Data.Models
{
    public class DbQuestion : Question
    {
        public DbRound Round { get; set; } = default!;
        public List<DbSubmittedAnswer> SubmittedAnswers { get; set; } = default!;
    }
}
