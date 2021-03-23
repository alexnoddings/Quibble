using Quibble.Shared.Models;

namespace Quibble.Server.Data.Models
{
    public class DbSubmittedAnswer : SubmittedAnswer
    {
        public DbQuestion Question { get; set; } = default!;
        public AppUser Submitter { get; set; } = default!;
    }
}
