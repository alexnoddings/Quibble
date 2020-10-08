using System;
using System.Threading.Tasks;
using Quibble.Host.Common.Data.Entities;

namespace Quibble.Host.Common.Repositories
{
    public interface IUserRepository
    {
        public Task<DbQuibbleUser> GetAsync(Guid id);
    }
}
