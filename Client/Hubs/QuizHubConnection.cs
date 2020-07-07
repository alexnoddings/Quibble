using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Quibble.Common.Protos;
using Quibble.Common.SignalR;

namespace Quibble.Client.Hubs
{
    /// <summary>
    /// A <see cref="HubConnection"/> for quizzes.
    /// </summary>
    public class QuizHubConnection : IAsyncDisposable, IInvokableQuizHub
    {
        private readonly HubConnection _hubConnection;

        /// <summary>
        /// Initialises a new instance of <see cref="QuizHubConnection"/>.
        /// </summary>
        /// <param name="hubConnectionInner">The inner <seealso cref="HubConnection"/> to use.</param>
        public QuizHubConnection(HubConnection hubConnectionInner)
        {
            _hubConnection = hubConnectionInner ?? throw new ArgumentNullException(nameof(hubConnectionInner));
        }

        /// <inheritdoc cref="HubConnection.StartAsync"/>
        public Task StartAsync(CancellationToken cancellationToken = default) =>
            _hubConnection.StartAsync(cancellationToken);

        /// <summary>
        /// Registers a handler to be invoked when a quiz is updated.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <returns>A subscription that can be disposed to unsubscribe from the hub method.</returns>
        /// <seealso cref="IQuizHubClient.OnQuizUpdated"/>
        public IDisposable OnQuizUpdated(Func<string, Task> handler) =>
            _hubConnection.On(nameof(IQuizHubClient.OnQuizUpdated), handler);

        /// <summary>
        /// Registers a handler to be invoked when a quiz is updated.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <returns>A subscription that can be disposed to unsubscribe from the hub method.</returns>
        /// <seealso cref="IQuizHubClient.OnQuizUpdated"/>
        public IDisposable OnQuizUpdated(Func<Task> handler) =>
            _hubConnection.On(nameof(IQuizHubClient.OnQuizUpdated), handler);

        /// <summary>
        /// Registers a handler to be invoked when a round is updated.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <returns>A subscription that can be disposed to unsubscribe from the hub method.</returns>
        /// <seealso cref="IQuizHubClient.OnRoundUpdated"/>
        public IDisposable OnRoundUpdated(Func<string, RoundState, Task> handler) =>
            _hubConnection.On(nameof(IQuizHubClient.OnRoundUpdated), handler);

        /// <summary>
        /// Registers a handler to be invoked when a round is updated.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <returns>A subscription that can be disposed to unsubscribe from the hub method.</returns>
        /// <seealso cref="IQuizHubClient.OnRoundUpdated"/>
        public IDisposable OnRoundUpdated(Func<Task> handler) =>
            _hubConnection.On(nameof(IQuizHubClient.OnRoundUpdated), handler);

        /// <summary>
        /// Registers a handler to be invoked when a question is updated.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <returns>A subscription that can be disposed to unsubscribe from the hub method.</returns>
        /// <seealso cref="IQuizHubClient.OnQuestionUpdated"/>
        public IDisposable OnQuestionUpdated(Func<string, string, QuestionState, Task> handler) =>
            _hubConnection.On(nameof(IQuizHubClient.OnQuestionUpdated), handler);

        /// <summary>
        /// Registers a handler to be invoked when a question is updated.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <returns>A subscription that can be disposed to unsubscribe from the hub method.</returns>
        /// <seealso cref="IQuizHubClient.OnQuestionUpdated"/>
        public IDisposable OnQuestionUpdated(Func<Task> handler) =>
            _hubConnection.On(nameof(IQuizHubClient.OnQuestionUpdated), handler);

        /// <inheritdoc cref="IInvokableQuizHub.RegisterToQuizUpdatesAsync"/>
        public Task RegisterToQuizUpdatesAsync(string id) =>
            _hubConnection.InvokeAsync(nameof(IInvokableQuizHub.RegisterToQuizUpdatesAsync), id);

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
