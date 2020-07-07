using System;
using Quibble.Server.Models.Users;

namespace Quibble.Server.Models.Quizzes
{
    /// <summary>
    /// Represents a quiz.
    /// </summary>
    public class Quiz : IEntity<Guid>
    {
        /// <inheritdoc />
        public Guid Id { get; set; }

        /// <summary>
        /// The identifier of the owner <see cref="ApplicationUser"/>.
        /// </summary>
        public string OwnerId { get; set; } = string.Empty;

        /// <summary>
        /// The title.
        /// </summary>
        public string Title { get; set; } = "New Quiz";

        /// <summary>
        /// The state.
        /// </summary>
        public QuizState State { get; set; } = QuizState.InDevelopment;
    }
}
