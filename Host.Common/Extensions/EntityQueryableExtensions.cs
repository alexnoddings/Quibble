using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Quibble.Core;

namespace Quibble.Host.Common.Extensions
{
    public static class EntityQueryableExtensions
    {
        public static Task<bool> ExistsAsync<TEntity>(this IQueryable<TEntity> source, Guid id)
            where TEntity : class, IEntity<Guid> =>
            source.AnyAsync(e => e.Id == id);

        public static Task<TEntity?> WithIdAsync<TEntity>(this IQueryable<TEntity> source, Guid id)
            where TEntity : class, IEntity<Guid> =>
            source.FirstOrDefaultAsync(e => e.Id == id)!;
    }
}
