using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace Quibble.Caching;

public static class CacheKey
{
    [SuppressMessage(
        "Design", "CA1000: Do not declare static members on generic types",
        Justification = "Makes consumption easier. Members are also static and pure."
    )]
    public static class For<TEntity>
    {
        [Pure]
        public static string WithId(Guid id)
        {
            var value = id.ToString("N");

            return WithProperty("id", value);
        }

        [Pure]
        public static string WithCompositeId(Guid id1, Guid id2)
        {
            var id1str = id1.ToString("N");
            var id2str = id2.ToString("N");

            var value = $"{id1str}+{id2str}";
            return WithProperty("compositeId", value);
        }

        [Pure]
        public static string WithProperty(string property, string value)
        {
            var entityName = typeof(TEntity).Name;

            return Create(entityName, property, value);
        }

        [Pure]
        private static string Create(string entityType, string property, string value) =>
            $"{entityType}:{property}:{value}";
    }
}

