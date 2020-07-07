using System;
using ProtosQuestionState = Quibble.Common.Protos.QuestionState;
using ModelsQuestionState = Quibble.Server.Models.Questions.QuestionState;

namespace Quibble.Server.Extensions.Models
{
    /// <summary>
    /// Extension methods for converting between <see cref="ProtosQuestionState"/> and <see cref="ModelsQuestionState"/>.
    /// </summary>
    public static class QuestionStateExtensions
    {
        /// <summary>
        /// Convert to a <see cref="ProtosQuestionState"/>.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>The converted state.</returns>
        public static ProtosQuestionState ToProtoEnum(this ModelsQuestionState state) =>
            state switch
            {
                ModelsQuestionState.Hidden => ProtosQuestionState.QuestionHidden,
                ModelsQuestionState.Available => ProtosQuestionState.QuestionAvailable,
                ModelsQuestionState.Locked => ProtosQuestionState.QuestionLocked,
                ModelsQuestionState.Released => ProtosQuestionState.QuestionReleased,
                _ => throw new ArgumentOutOfRangeException(nameof(state))
            };

        /// <summary>
        /// Convert to a <see cref="ModelsQuestionState"/>.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>The converted state.</returns>
        public static ModelsQuestionState ToModelEnum(this ProtosQuestionState state) =>
            state switch
            {
                ProtosQuestionState.QuestionHidden => ModelsQuestionState.Hidden,
                ProtosQuestionState.QuestionAvailable => ModelsQuestionState.Available,
                ProtosQuestionState.QuestionLocked => ModelsQuestionState.Locked,
                ProtosQuestionState.QuestionReleased => ModelsQuestionState.Released,
                _ => throw new ArgumentOutOfRangeException(nameof(state))
            };
    }
}
