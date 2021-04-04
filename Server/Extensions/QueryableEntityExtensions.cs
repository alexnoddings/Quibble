using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Quibble.Shared.Models;

namespace Quibble.Server.Extensions
{
    public static class QueryableEntityExtensions
    {
        public static Task<TEntity> FindAsync<TId, TEntity>(this IQueryable<TEntity> source, TId id)
            where TId : IEquatable<TId>
            where TEntity : IEntity<TId> =>
            source.FirstOrDefaultAsync(entity => entity.Id.Equals(id));
    }
}
