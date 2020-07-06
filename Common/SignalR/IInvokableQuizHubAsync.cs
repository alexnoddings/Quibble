using System.Threading.Tasks;

namespace Quibble.Common.SignalR
{
    /// <summary>
    /// Provides the actions a user can invoke on the Quiz SignalR hub.
    /// </summary>
    public interface IInvokableQuizHub
    {
        /// <summary>
        /// Registers a user to receive updates for quiz with id <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the quiz.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous register operation.</returns>
        Task RegisterToQuizUpdatesAsync(string id);
    }
}
