using System;
using ProtosRoundState = Quibble.Common.Protos.RoundState;
using ModelsRoundState = Quibble.Server.Models.Rounds.RoundState;

namespace Quibble.Server.Extensions.Models
{
    /// <summary>
    /// Extension methods for converting between <see cref="ProtosRoundState"/> and <see cref="ModelsRoundState"/>.
    /// </summary>
    public static class RoundStateExtensions
    {
        /// <summary>
        /// Convert to a <see cref="ProtosRoundState"/>.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>The converted state.</returns>
        public static ProtosRoundState ToProtoEnum(this ModelsRoundState state) =>
            state switch
            {
                ModelsRoundState.Available => ProtosRoundState.RoundAvailable,
                ModelsRoundState.Hidden => ProtosRoundState.RoundHidden,
                _ => throw new ArgumentOutOfRangeException(nameof(state))
            };

        /// <summary>
        /// Convert to a <see cref="ModelsRoundState"/>.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>The converted state.</returns>
        public static ModelsRoundState ToModelEnum(this ProtosRoundState state) =>
            state switch
            {
                ProtosRoundState.RoundAvailable => ModelsRoundState.Available,
                ProtosRoundState.RoundHidden => ModelsRoundState.Hidden,
                _ => throw new ArgumentOutOfRangeException(nameof(state))
            };
    }
}
