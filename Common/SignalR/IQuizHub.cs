using System.Threading.Tasks;

namespace Quibble.Common.SignalR
{
    /// <summary>
    /// Provides the actions available to perform on clients of the Quiz SignalR hub.
    /// </summary>
    public interface IQuizHub
    {
        /// <summary>
        /// Called when a Quiz's title is updated.
        /// </summary>
        /// <param name="newTitle">The new title.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task OnQuizTitleUpdated(string newTitle);
    }
}
