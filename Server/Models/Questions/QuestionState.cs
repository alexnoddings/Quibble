using Quibble.Server.Models.Participants;
using Quibble.Server.Models.Quizzes;

namespace Quibble.Server.Models.Questions
{
    /// <summary>
    /// Represents what state a <see cref="Question"/> is in.
    /// </summary>
    public enum QuestionState
    {
        /// <summary>
        /// The <see cref="Question"/> is hidden to <see cref="Participant"/>s.
        /// </summary>
        Hidden,

        /// <summary>
        /// The <see cref="Question.Body"/> is visible to <see cref="Participant"/>s and they may submit a <see cref="SubmittedAnswer"/>.
        /// </summary>
        Available,

        /// <summary>
        /// The <see cref="Question"/> is locked and <see cref="SubmittedAnswer"/>s may not be submitted or edited.
        /// The correctness of <see cref="SubmittedAnswer"/>s can be decided by the <see cref="Quiz"/> owner.
        /// </summary>
        Locked,

        /// <summary>
        /// The <see cref="Question.Answer"/> is released and <see cref="Participant"/>s can see if they were right or wrong.
        /// </summary>
        Released
    }
}
