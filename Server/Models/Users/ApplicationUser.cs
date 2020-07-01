using System;
using Microsoft.AspNetCore.Identity;

namespace Quibble.Server.Models.Users
{
    public class ApplicationUser : IdentityUser, IEntity<string>
    {
    }
}
