using System.Collections.Generic;
using Quibble.Shared.Models;

namespace Quibble.Server.Data.Models
{
    public class DbQuiz : Quiz
    {
        public AppUser Owner { get; set; } = default!;
        public List<DbRound> Rounds { get; set; } = default!;
    }
}
