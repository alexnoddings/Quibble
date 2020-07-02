using System;
using Quibble.Server.Models.Quizzes;
using Quibble.Server.Models.Users;

namespace Quibble.Server.Models.Participants
{
    /// <summary>
    /// Represents a <see cref="ApplicationUser"/>'s participation in a <see cref="Quiz"/>.
    /// </summary>
    public class Participant : IEntity<Guid>
    {
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The identifier of the <see cref="ApplicationUser"/> represented.
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// The identifier of the <see cref="Quiz"/> being taken.
        /// </summary>
        public Guid QuizId { get; set; }

        /// <summary>
        /// The displayed nickname.
        /// </summary>
        public string NickName { get; set; } = "User";
    }
}
 