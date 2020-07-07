using Microsoft.AspNetCore.SignalR.Client;
using Quibble.Client.Hubs;

namespace Quibble.Client.Extensions.SignalR
{
    /// <summary>
    /// Extension methods for <see cref="HubConnection"/>.
    /// </summary>
    /// <remarks>
    /// These methods are useful for chaining calls during <see cref="HubConnection"/> building.
    /// </remarks>
    public static class HubConnectionExtensions
    {
        /// <summary>
        /// Initialises a new <see cref="QuizHubConnection"/> from the <see cref="HubConnection"/>.
        /// </summary>
        /// <param name="hubConnection">The <see cref="HubConnection"/>.</param>
        /// <returns>The <see cref="QuizHubConnection"/>.</returns>
        public static QuizHubConnection AsQuizHubConnection(this HubConnection hubConnection) =>
            new QuizHubConnection(hubConnection);
    }
}
