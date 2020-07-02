using System;
using Quibble.Server.Models.Questions;

namespace Quibble.Server.Models.Participants
{
    /// <summary>
    /// Represents an answer submitted by a <see cref="Participant"/> to a <see cref="Question"/>.
    /// </summary>
    public class SubmittedAnswer : IEntity<Guid>
    {
        /// <inheritdoc />
        public Guid Id { get; }

        /// <summary>
        /// The identifier of the <see cref="Participant"/> who submitted this.
        /// </summary>
        public Guid ParticipantId { get; set; }

        /// <summary>
        /// The identifier of the <see cref="Question"/> which this is for.
        /// </summary>
        public Guid QuestionId { get; set; }

        /// <summary>
        /// The state.
        /// </summary>
        public SubmittedAnswerState State { get; set; } = SubmittedAnswerState.Unmarked;

        /// <summary>
        /// The text body of the answer.
        /// </summary>
        public string Value { get; set; } = string.Empty;
    }
}
