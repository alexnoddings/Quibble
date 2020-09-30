using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using Quibble.Host.Common.Data.Entities;

namespace Quibble.Host.Common
{
    [Pure]
    public static class Ensure
    {
        public static void NotNullOrDefault<T>([AllowNull][NotNull] T? obj, string paramName, string? message = null)
        {
            if (obj is null)
                throw ThrowHelper.NullArgument(paramName, message);
            if (obj.Equals(default(T)))
                throw ThrowHelper.BadArgument(paramName, message);
        }

        public static void Found([AllowNull][NotNull] object? obj, string typeName, object? id = null, string? message = null)
        {
            if (obj == null)
                throw ThrowHelper.NotFound(typeName, id, message);
        }

        public static void Authenticated([AllowNull][NotNull] DbQuibbleUser? user, string? message = null)
        {
            if (user == null)
                throw ThrowHelper.Unauthenticated(message);
        }
    }
}
