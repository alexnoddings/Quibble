using System;
using System.ComponentModel.DataAnnotations;

namespace Quibble.Core
{
    /// <summary>
    ///     Represents an entity with an <c>Id</c>.
    /// </summary>
    /// <typeparam name="TKey">
    ///     The type to use for the <c>Id</c>.
    /// </typeparam>
    public interface IEntity<out TKey> where TKey : IEquatable<TKey>
    {
        [Key]
        public TKey Id { get; }
    }

    /// <summary>
    ///     Represents an entity with an <c>Id</c> of type <see cref="Guid"/>.
    /// </summary>
    /// <inheritdoc cref="IEntity{TKey}"/>
    public interface IEntity : IEntity<Guid>
    {
    }
}