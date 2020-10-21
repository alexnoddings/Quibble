using System;
using System.Threading.Tasks;
using Quibble.Core.Exceptions;
using Quibble.Host.Common.Data.Entities;
using Quibble.Host.Common.Services.UserContextAccessor;

namespace Quibble.Host.Common.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IUserContextAccessor" />.
    /// </summary>
    public static class UserContextAccessorExtensions
    {
        /// <summary>
        ///     Ensures that a user is currently associated with the context.
        /// </summary>
        /// <param name="userContextAccessor">The <see cref="IUserContextAccessor"/>.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains the logged in <see cref="DbQuibbleUser"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="userContextAccessor"/> is null.
        /// </exception>
        /// <exception cref="UnauthenticatedException">
        ///     The context does not have an authenticated user associated with it.
        /// </exception>
        public static async Task<DbQuibbleUser> EnsureCurrentUserAsync(this IUserContextAccessor userContextAccessor)
        {
            Ensure.NotNullOrDefault(userContextAccessor, nameof(userContextAccessor));
            DbQuibbleUser? user = await userContextAccessor.GetCurrentUserAsync();
            Ensure.Authenticated(user);
            return user;
        }
    }
}
