using System;
using System.Security.Claims;

namespace Quibble.Core.Extensions
{
    public static class ClaimsPrincipleExtensions
    {
        public static Guid GetId(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal == null) throw new ArgumentNullException(nameof(claimsPrincipal));

            var idStr = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.Parse(idStr);
        }
    }
}
