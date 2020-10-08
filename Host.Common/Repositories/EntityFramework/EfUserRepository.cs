using System;
using System.Threading.Tasks;
using Quibble.Host.Common.Data;
using Quibble.Host.Common.Data.Entities;
using Quibble.Host.Common.Extensions;

namespace Quibble.Host.Common.Repositories.EntityFramework
{
    internal class EfUserRepository : IUserRepository
    {
        private IQuibbleDbContext DbContext { get; }

        public EfUserRepository(IQuibbleDbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<DbQuibbleUser> GetAsync(Guid id)
        {
            Ensure.NotNullOrDefault(id, nameof(id));
            DbQuibbleUser? user = await DbContext.Users.WithIdAsync(id);
            Ensure.Found(user, "User", id);
            return user;
        }
    }
}
