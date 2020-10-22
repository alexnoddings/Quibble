using System.Threading.Tasks;
using Quibble.Host.Common.Data.Entities;

namespace Quibble.Host.Common.Services.UserContextAccessor
{
    /// <summary>
    ///     Gets the <see cref="DbQuibbleUser"/> from the current context.
    /// </summary>
    public interface IUserContextAccessor
    {
        /// <summary>
        ///     Gets the <see cref="DbQuibbleUser"/> from the current context, or null if there is no user associated with the context.
        /// </summary>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains the logged in <see cref="DbQuibbleUser"/> or null.
        /// </returns>
        public Task<DbQuibbleUser?> GetCurrentUserAsync();
    }
}
