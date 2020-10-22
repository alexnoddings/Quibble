using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Quibble.Core.Extensions;
using Quibble.Host.Common.Data;
using Quibble.Host.Common.Data.Entities;
using Quibble.Host.Common.Extensions;

namespace Quibble.Host.Common.Services.UserContextAccessor
{
    /// <summary>
    ///     Gets the <see cref="DbQuibbleUser"/> from the current context from Entity Framework.
    /// </summary>
    internal class EntityFrameworkUserContextAccessor : IUserContextAccessor
    {
        private IHttpContextAccessor HttpContextAccessor { get; }
        private IQuibbleDbContext DbContext { get; }
        private ILogger<EntityFrameworkUserContextAccessor> Logger { get; }

        public EntityFrameworkUserContextAccessor(IHttpContextAccessor httpContextAccessor, IQuibbleDbContext dbContext, ILogger<EntityFrameworkUserContextAccessor> logger)
        {
            HttpContextAccessor = httpContextAccessor;
            DbContext = dbContext;
            Logger = logger;
        }

        /// <inheritdoc />
        public async Task<DbQuibbleUser?> GetCurrentUserAsync()
        {
            ClaimsPrincipal? user = HttpContextAccessor.HttpContext?.User;
            if (user?.Identity == null)
            {
                Logger.LogDebug("Attempt to get current user without an identity.");
                return null;
            }

            Guid userId = user.GetId();
            DbQuibbleUser? dbUser = await DbContext.Users.WithIdAsync(userId);
            if (dbUser == null)
            {
                Logger.LogWarning($"HttpContext gave user ID as {userId} but no user has this ID.");
                return null;
            }

            Logger.LogTrace($"Found user {dbUser.Id} [{dbUser.UserName}].");
            return dbUser;
        }
    }
}
