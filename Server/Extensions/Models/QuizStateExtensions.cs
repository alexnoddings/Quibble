using System;
using ProtosQuizState = Quibble.Common.Protos.QuizState;
using ModelsQuizState = Quibble.Server.Models.Quizzes.QuizState;

namespace Quibble.Server.Extensions.Models
{
    /// <summary>
    /// Extension methods for converting between <see cref="ProtosQuizState"/> and <see cref="ModelsQuizState"/>.
    /// </summary>
    public static class QuizStateExtensions
    {
        /// <summary>
        /// Convert to a <see cref="ProtosQuizState"/>.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>The converted state.</returns>
        public static ProtosQuizState ToProtoEnum(this ModelsQuizState state) =>
            state switch
            {
                ModelsQuizState.Closed => ProtosQuizState.Closed,
                ModelsQuizState.InDevelopment => ProtosQuizState.InDevelopment,
                ModelsQuizState.InProgress => ProtosQuizState.InProgress,
                ModelsQuizState.WaitingForPlayers => ProtosQuizState.WaitingForPlayers,
                _ => throw new ArgumentOutOfRangeException(nameof(state))
            };

        /// <summary>
        /// Convert to a <see cref="ModelsQuizState"/>.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>The converted state.</returns>
        public static ModelsQuizState ToModelEnum(this ProtosQuizState state) =>
            state switch
            {
                ProtosQuizState.Closed => ModelsQuizState.Closed,
                ProtosQuizState.InDevelopment => ModelsQuizState.InDevelopment,
                ProtosQuizState.InProgress => ModelsQuizState.InProgress,
                ProtosQuizState.WaitingForPlayers => ModelsQuizState.WaitingForPlayers,
                _ => throw new ArgumentOutOfRangeException(nameof(state))
            };
    }
}
