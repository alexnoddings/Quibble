using System;
using Quibble.Server.Models.Participants;
using Quibble.Server.Models.Quizzes;
using Quibble.Server.Models.Rounds;

namespace Quibble.Server.Models.Questions
{
    /// <summary>
    /// Represents a question in a <see cref="Quiz"/>.
    /// </summary>
    public class Question : IEntity<Guid>
    {
        /// <inheritdoc />
        public Guid Id { get; }

        /// <summary>
        /// The identifier of the parent <see cref="Round"/>.
        /// </summary>
        public Guid RoundId { get; set; }

        /// <summary>
        /// The text body of the question displayed to <see cref="Participant"/>s.
        /// </summary>
        public string Body { get; set; } = string.Empty;

        /// <summary>
        /// The correct answer to the question displayed to the owner and when the answer is released.
        /// </summary>
        public string Answer { get; set; } = string.Empty;

        /// <summary>
        /// The state.
        /// </summary>
        public QuestionState State { get; set; } = QuestionState.Hidden;
    }
}
