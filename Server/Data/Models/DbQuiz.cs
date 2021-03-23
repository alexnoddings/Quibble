using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quibble.Shared.Models;

namespace Quibble.Server.Data.Models
{
    public class DbQuiz : Quiz
    {
        public AppUser Owner { get; set; } = default!;
        public List<Round> Rounds { get; set; } = default!;
    }
}
