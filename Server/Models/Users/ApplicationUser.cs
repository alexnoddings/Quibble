using Microsoft.AspNetCore.Identity;

namespace Quibble.Server.Models.Users
{
    /// <summary>
    /// Represents a user of the system.
    /// </summary>
    public class ApplicationUser : IdentityUser, IEntity<string>
    {
    }
}
