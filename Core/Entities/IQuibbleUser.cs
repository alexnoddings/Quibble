using System;

namespace Quibble.Core.Entities
{
    public interface IQuibbleUser : IEntity
    {
        public string UserName { get; }
        public DateTime Joined { get; }
    }
}