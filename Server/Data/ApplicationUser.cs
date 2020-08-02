using Microsoft.AspNetCore.Identity;
using Quibble.Common;

namespace Quibble.Server.Data
{
    /// <summary>
    /// Represents a user of the system.
    /// </summary>
    public class ApplicationUser : IdentityUser, IEntity<string>
    {
    }
}
