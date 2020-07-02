using System;
using Quibble.Server.Models.Questions;
using Quibble.Server.Models.Quizzes;

namespace Quibble.Server.Models.Rounds
{
    /// <summary>
    /// Represents a round of <see cref="Question"/>s in a <see cref="Quiz"/>.
    /// </summary>
    public class Round : IEntity<Guid>
    {
        /// <inheritdoc />
        public Guid Id { get; }

        /// <summary>
        /// The identifier of the parent <see cref="Quiz"/>.
        /// </summary>
        public Guid QuizId { get; }

        /// <summary>
        /// The title.
        /// </summary>
        public string Title { get; set; } = "New Round";

        /// <summary>
        /// The state.
        /// </summary>
        public RoundState State { get; set; } = RoundState.Hidden;
    }
}
