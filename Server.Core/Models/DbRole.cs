using Microsoft.AspNetCore.Identity;
using Quibble.Common.Entities;

namespace Quibble.Server.Core.Models;

public class DbRole : IdentityRole<Guid>, IEntity
{
}
