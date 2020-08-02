using System;
using System.Security.Claims;

namespace Quibble.Client.Extensions.Identity
{
    /// <summary>
    /// Extension methods for <see cref="ClaimsPrincipal"/>s.
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Gets the <see cref="ClaimTypes.NameIdentifier"/> of a user.
        /// </summary>
        /// <param name="claimsPrincipal">The <see cref="ClaimsPrincipal"/>.</param>
        /// <returns>The user's identifier.</returns>
        internal static string GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal == null) throw new ArgumentNullException(nameof(claimsPrincipal));

            // IdentityServer seems to store the Id in a Claim type "sub" instead of ClaimTypes.NameIdentifier
            return claimsPrincipal.FindFirst("sub")?.Value ?? string.Empty;
        }
    }
}
