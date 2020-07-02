using Quibble.Server.Models.Participants;

namespace Quibble.Server.Models.Rounds
{
    /// <summary>
    /// Represents what state a <see cref="Round"/> is in.
    /// </summary>
    public enum RoundState
    {
        /// <summary>
        /// The <see cref="Round"/> is hidden to <see cref="Participant"/>s.
        /// </summary>
        Hidden,

        /// <summary>
        /// The <see cref="Round"/> is available to <see cref="Participant"/>s.
        /// This does not mean that any questions are yet.
        /// </summary>
        Available
    }
}
