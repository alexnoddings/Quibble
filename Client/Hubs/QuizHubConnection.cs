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

        /// <inheritdoc cref="IInvokableQuizHub.RegisterToQuizUpdatesAsync"/>
        public Task RegisterToQuizUpdatesAsync(string id) =>
            _hubConnection.InvokeAsync(nameof(IInvokableQuizHub.RegisterToQuizUpdatesAsync), id);

        /// <inheritdoc cref="HubConnection.StartAsync"/>
        public Task StartAsync(CancellationToken cancellationToken = default) =>
            _hubConnection.StartAsync(cancellationToken);

        /// <summary>
        /// Registers a handler to be invoked when a Quiz is updated.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <returns>A subscription that can be disposed to unsubscribe from the hub method.</returns>
        /// <seealso cref="IQuizHubClient.OnQuizUpdated"/>
        public IDisposable OnQuizUpdated(Func<QuizInfo, Task> handler) =>
            _hubConnection.On(nameof(IQuizHubClient.OnQuizUpdated), handler);

        /// <summary>
        /// Registers a handler to be invoked when a Quiz is deleted.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <returns>A subscription that can be disposed to unsubscribe from the hub method.</returns>
        /// <seealso cref="IQuizHubClient.OnQuizDeleted"/>
        public IDisposable OnQuizDeleted(Func<string, Task> handler) =>
            _hubConnection.On(nameof(IQuizHubClient.OnQuizDeleted), handler);

        /// <summary>
        /// Registers a handler to be invoked when a Round is created.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <returns>A subscription that can be disposed to unsubscribe from the hub method.</returns>
        /// <seealso cref="IQuizHubClient.OnRoundCreated"/>
        public IDisposable OnRoundCreated(Func<RoundInfo, Task> handler) =>
            _hubConnection.On(nameof(IQuizHubClient.OnRoundCreated), handler);

        /// <summary>
        /// Registers a handler to be invoked when a Round is updated.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <returns>A subscription that can be disposed to unsubscribe from the hub method.</returns>
        /// <seealso cref="IQuizHubClient.OnRoundUpdated"/>
        public IDisposable OnRoundUpdated(Func<RoundInfo, Task> handler) =>
            _hubConnection.On(nameof(IQuizHubClient.OnRoundUpdated), handler);

        /// <summary>
        /// Registers a handler to be invoked when a Round is deleted.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <returns>A subscription that can be disposed to unsubscribe from the hub method.</returns>
        /// <seealso cref="IQuizHubClient.OnRoundDeleted"/>
        public IDisposable OnRoundDeleted(Func<string, Task> handler) =>
            _hubConnection.On(nameof(IQuizHubClient.OnRoundDeleted), handler);

        /// <summary>
        /// Registers a handler to be invoked when a Question is created.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <returns>A subscription that can be disposed to unsubscribe from the hub method.</returns>
        /// <seealso cref="IQuizHubClient.OnQuestionCreated"/>
        public IDisposable OnQuestionCreated(Func<QuestionInfo, Task> handler) =>
            _hubConnection.On(nameof(IQuizHubClient.OnQuestionCreated), handler);

        /// <summary>
        /// Registers a handler to be invoked when a Question is updated.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <returns>A subscription that can be disposed to unsubscribe from the hub method.</returns>
        /// <seealso cref="IQuizHubClient.OnQuestionUpdated"/>
        public IDisposable OnQuestionUpdated(Func<QuestionInfo, Task> handler) =>
            _hubConnection.On(nameof(IQuizHubClient.OnQuestionUpdated), handler);

        /// <summary>
        /// Registers a handler to be invoked when a Question is deleted.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <returns>A subscription that can be disposed to unsubscribe from the hub method.</returns>
        /// <seealso cref="IQuizHubClient.OnQuestionDeleted"/>
        public IDisposable OnQuestionDeleted(Func<string, string, Task> handler) =>
            _hubConnection.On(nameof(IQuizHubClient.OnQuestionDeleted), handler);

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
