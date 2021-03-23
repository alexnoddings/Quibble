using System;
using Microsoft.AspNetCore.Identity;
using Quibble.Shared.Models;

namespace Quibble.Server.Data
{
    public class AppRole : IdentityRole<Guid>, IEntity
    {
    }
}
