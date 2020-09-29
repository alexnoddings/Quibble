using System;
using System.Threading.Tasks;
using Quibble.Core;

namespace Quibble.Host.Common.Repositories
{
    public interface IEntityRepository<TId, TEntity>
        where TId : IEquatable<TId>
        where TEntity : IEntity<TId>
    {
        public Task<TId> CreateAsync(TEntity entity);
        public Task<bool> ExistsAsync(TId id);
        public Task<TEntity> GetAsync(TId id);
        public Task DeleteAsync(TId id);
    }
}
