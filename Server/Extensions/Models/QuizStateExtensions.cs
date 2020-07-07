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
                ModelsQuizState.Closed => ProtosQuizState.QuizClosed,
                ModelsQuizState.InDevelopment => ProtosQuizState.QuizInDevelopment,
                ModelsQuizState.InProgress => ProtosQuizState.QuizInProgress,
                ModelsQuizState.WaitingForPlayers => ProtosQuizState.QuizWaitingForPlayers,
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
                ProtosQuizState.QuizClosed => ModelsQuizState.Closed,
                ProtosQuizState.QuizInDevelopment => ModelsQuizState.InDevelopment,
                ProtosQuizState.QuizInProgress => ModelsQuizState.InProgress,
                ProtosQuizState.QuizWaitingForPlayers => ModelsQuizState.WaitingForPlayers,
                _ => throw new ArgumentOutOfRangeException(nameof(state))
            };
    }
}
