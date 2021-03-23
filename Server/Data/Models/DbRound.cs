using System.Collections.Generic;
using Quibble.Shared.Models;

namespace Quibble.Server.Data.Models
{
    public class DbRound : Round
    {
        public DbQuiz Quiz { get; set; } = default!;
        public List<DbQuestion> Questions { get; set; } = default!;
    }
}
