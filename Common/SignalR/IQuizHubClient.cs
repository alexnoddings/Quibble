using System.Threading.Tasks;
using Quibble.Common.Protos;

namespace Quibble.Common.SignalR
{
    /// <summary>
    /// Provides the actions available to perform on clients of the Quiz SignalR hub.
    /// </summary>
    public interface IQuizHubClient
    {
        /// <summary>
        /// Called when a Quiz is updated.
        /// </summary>
        /// <param name="newTitle">The new title.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous updating operation.</returns>
        Task OnQuizUpdated(string newTitle);

        /// <summary>
        /// Called when a Round is updated.
        /// </summary>
        /// <param name="newTitle">The new title.</param>
        /// <param name="newState">The new state.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous updating operation.</returns>
        Task OnRoundUpdated(string newTitle, RoundState newState);

        /// <summary>
        /// Called when a Question is updated.
        /// </summary>
        /// <param name="newBody">The new body.</param>
        /// <param name="newAnswer">The new answer.</param>
        /// <param name="newState">The new state.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous updating operation.</returns>
        Task OnQuestionUpdated(string newBody, string newAnswer, QuestionState newState);
    }
}
