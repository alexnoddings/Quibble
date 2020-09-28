using System;
using System.ComponentModel.DataAnnotations;

namespace Quibble.Core
{
    public interface IEntity<TKey> where TKey : IEquatable<TKey>
    {
        [Key]
        public TKey Id { get; }
    }

    public interface IEntity : IEntity<Guid>
    {
    }
}