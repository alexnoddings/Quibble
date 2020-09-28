using System;

namespace Quibble.Tests.Host.Common
{
    /// <summary>
    ///     A simple object used in tests.
    /// </summary>
    public class SimpleObject : IEquatable<SimpleObject>
    {
        /// <summary>
        ///     This object's Id.
        ///     This is randomly generated for each <see cref="SimpleObject"/>.
        /// </summary>
        public Guid Id { get; init; } = Guid.NewGuid();

        public bool Equals(SimpleObject? other)
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
            return Equals((SimpleObject) obj);
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}
