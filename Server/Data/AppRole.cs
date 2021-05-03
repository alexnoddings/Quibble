using System;
using Microsoft.AspNetCore.Identity;
using Quibble.Shared.Entities;

namespace Quibble.Server.Data
{
    public class AppRole : IdentityRole<Guid>, IEntity
    {
    }
}
