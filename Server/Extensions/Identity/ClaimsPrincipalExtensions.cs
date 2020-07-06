﻿using System.Security.Claims;

namespace Quibble.Server.Extensions.Identity
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
        public static string GetUserId(this ClaimsPrincipal claimsPrincipal) =>
            claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}