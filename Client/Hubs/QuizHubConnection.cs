using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Quibble.Common.SignalR;

namespace Quibble.Client.Hubs
{
    /// <summary>
    /// A <see cref="HubConnection"/> for quizzes.
    /// </summary>
    public class QuizHubConnection : IAsyncDisposable, IInvokableQuizHub
    {
        private readonly HubConnection _hubConnection;

        public QuizHubConnection(HubConnection hubConnectionInner)
        {
            _hubConnection = hubConnectionInner ?? throw new ArgumentNullException(nameof(hubConnectionInner));
        }

        /// <inheritdoc cref="HubConnection.StartAsync"/>
        public Task StartAsync(CancellationToken cancellationToken = default) =>
            _hubConnection.StartAsync(cancellationToken);

        public IDisposable OnQuizTitleUpdated(Func<string, Task> handler)
        {
            return _hubConnection.On(nameof(IQuizHubClient.OnQuizTitleUpdated), handler);
        }

        public IDisposable OnQuizTitleUpdated(Func<Task> handler)
        {
            return _hubConnection.On(nameof(IQuizHubClient.OnQuizTitleUpdated), handler);
        }

        /// <inheritdoc cref="IInvokableQuizHub.RegisterToQuizUpdatesAsync"/>
        public Task RegisterToQuizUpdatesAsync(string id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Disposes the <see cref="QuizHubConnection"/>.
        /// </summary>
        /// <returns>A <see cref="ValueTask"/> that represents the asynchronous disposal operation.</returns>
        public async ValueTask DisposeAsync()
        {
            await _hubConnection.DisposeAsync().ConfigureAwait(false);
        }
    }
}
