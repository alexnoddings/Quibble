using System;

namespace Quibble.Server.Models.Users
{
    /// <summary>
    /// Represents the settings of a <see cref="ApplicationUser"/>.
    /// </summary>
    public class UserSettings : IEntity<Guid>
    {
        /// <inheritdoc />
        public Guid Id { get; }

        /// <summary>
        /// The identifier of the <see cref="ApplicationUser"/>.
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Whether the <see cref="ApplicationUser"/> is using night mode.
        /// </summary>
        public bool UseNightMode { get; set; }
    }
}
