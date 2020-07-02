using Quibble.Server.Models.Questions;

namespace Quibble.Server.Models.Participants
{
    /// <summary>
    /// Represents what state a <see cref="SubmittedAnswer"/> is in.
    /// </summary>
    public enum SubmittedAnswerState
    {
        /// <summary>
        /// The <see cref="SubmittedAnswer"/> has not yet been marked.
        /// </summary>
        Unmarked,

        /// <summary>
        /// The <see cref="Participant"/> joined after the <see cref="Question.Answer"/> was released.
        /// It is not counted towards the score or any statistics as they were not wrong.
        /// </summary>
        NotSubmittedInTime,

        /// <summary>
        /// The <see cref="SubmittedAnswer"/> was wrong.
        /// </summary>
        Wrong,

        /// <summary>
        /// The <see cref="SubmittedAnswer"/> was right.
        /// </summary>
        Right
    }
}
