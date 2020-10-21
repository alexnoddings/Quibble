using System;
using System.Diagnostics.CodeAnalysis;
using Quibble.Core;
using Quibble.Core.Exceptions;
using Quibble.Host.Common.Data.Entities;

namespace Quibble.Host.Common
{

    /// <summary>
    ///     Contains various static methods that are used to ensure that conditions are met, otherwise an exception will be thrown.
    /// </summary>
    public static class Ensure
    {
        /// <summary>
        ///     Ensures that an obj is not null or default.
        ///     Will return <paramref name="obj"/> if it is not null or default, else it will throw an exception.
        /// </summary>
        /// <typeparam name="T">The type of object to check.</typeparam>
        /// <param name="obj">The object to check.</param>
        /// <param name="paramName">The name of the parameter which <paramref name="obj"/> comes from. Used for the exception message.</param>
        /// <param name="message">An optional message to include in the exception.</param>
        /// <returns><paramref name="obj"/> so long as it is not null or default.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is a reference type and is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="obj"/> is not a reference type and is default.</exception>
        public static T NotNullOrDefault<T>([AllowNull][NotNull] T? obj, string paramName, string? message = null)
        {
            if (obj is null)
                throw ThrowHelper.NullArgument(paramName, message);
            if (obj.Equals(default(T)))
                throw ThrowHelper.BadArgument(paramName, message);
            return obj;
        }

        /// <summary>
        ///     Ensures that an entity is found (i.e. is not null).
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TId">The type of the Id of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="typeName">The entity type name. Used for the exception message.</param>
        /// <param name="id">The id of the entity. Used for the exception message.</param>
        /// <param name="message">An optional message to include in the exception.</param>
        /// <returns><paramref name="entity"/> so long as it is not null or default.</returns>
        /// <exception cref="NotFoundException">Thrown when <paramref name="entity"/> is null.</exception>
        public static TEntity Found<TEntity, TId>([AllowNull][NotNull] TEntity? entity, string typeName, TId? id, string? message = null)
            where TEntity : IEntity<TId> 
            where TId : IEquatable<TId>
        {
            if (entity == null)
                throw ThrowHelper.NotFound(typeName, id, message);
            return entity;
        }

        /// <summary>
        ///     Ensures at a user is authenticated (i.e. exists, so it not null).
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="message">An optional message to include in the exception.</param>
        /// <exception cref="UnauthenticatedException">Thrown when <paramref name="user"/> is null.</exception>
        public static void Authenticated([AllowNull][NotNull] DbQuibbleUser? user, string? message = null)
        {
            if (user == null)
                throw ThrowHelper.Unauthenticated(message);
        }
    }
}
