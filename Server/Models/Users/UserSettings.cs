using System;

namespace Quibble.Server.Models.Users
{
    public class UserSettings : IEntity<Guid>
    {
        public Guid Id { get; }
        public string UserId { get; set; } = string.Empty;

        public bool UseNightMode { get; set; }
    }
}
