using System;
using System.Diagnostics.Contracts;
using Quibble.Core.Exceptions;

namespace Quibble.Host.Common
{
    [Pure]
    public static class ThrowHelper
    {
        public static ArgumentNullException NullArgument(string paramName, string? message = null) =>
            new ArgumentNullException(paramName, message);

        public static NotFoundException NotFound(string? typeName = null, string? id = null, string? message = null, Exception? innerException = null) =>
            new NotFoundException(typeName ?? "Entity", id, message, innerException);

        public static NotFoundException NotFound(string? typeName = null, object? id = null, string? message = null, Exception? innerException = null) =>
            NotFound(typeName ?? "Entity", id?.ToString(), message, innerException);

        public static UnauthorisedException Unauthorised(string? message = null, Exception? innerException = null) =>
            new UnauthorisedException(message, innerException);

        public static UnauthenticatedException Unauthenticated(string? message = null, Exception? innerException = null) =>
            new UnauthenticatedException(message, innerException);

        public static InvalidOperationException InvalidOperation(string? message = null, Exception? innerException = null) =>
            new InvalidOperationException(message, innerException);
    }
}
