using System;

namespace Quibble.Common
{
    public interface IEntity<TId>
    {
        public TId Id { get; set; }
    }

    public interface IEntity : IEntity<Guid>
    {
    }
}
