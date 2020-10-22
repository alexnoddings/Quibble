using System;
using Quibble.Core;

namespace Quibble.Tests.Host.Common
{
    /// <summary>
    ///     A simple entity used in tests.
    /// </summary>
    public class SimpleEntity : IEquatable<SimpleEntity>, IEntity
    {
        /// <remarks>
        ///     This is randomly generated for each <see cref="SimpleEntity"/>.
        /// </remarks>
        /// <inheritdoc />
        public Guid Id { get; init; } = Guid.NewGuid();

        public bool Equals(SimpleEntity? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SimpleEntity) obj);
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}
