using System;

namespace Quibble.Server.Models.Users
{
    public class UserSettings : IEntity<Guid>
    {
        public Guid Id { get; }
        public Guid UserId { get; set; }

        public bool UseNightMode { get; set; }
    }
}
