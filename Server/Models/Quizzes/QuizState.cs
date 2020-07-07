using Quibble.Server.Models.Participants;

namespace Quibble.Server.Models.Quizzes
{
    /// <summary>
    /// Represents what state a <see cref="Quiz"/> is in.
    /// </summary>
    public enum QuizState
    {
        /// <summary>
        /// The <see cref="Quiz"/> is being created by the owner.
        /// </summary>
        InDevelopment,

        /// <summary>
        /// <see cref="Participant"/>s are able to join the <see cref="Quiz"/>, but not yet access it.
        /// </summary>
        WaitingForPlayers,

        /// <summary>
        /// The <see cref="Quiz"/> is being done and <see cref="Participant"/>s are able to access it.
        /// </summary>
        InProgress,

        /// <summary>
        /// The <see cref="Quiz"/> has been completed and is now closed.
        /// </summary>
        Closed
    }
}
