using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Contexts;
using Quibble.Shared.Sync.SignalR;

namespace Quibble.Client.Sync.SignalR.Contexts
{
    internal class SignalrQuizSyncContext : BaseSignalrSyncContext, IQuizSyncContext
    {
        public event Func<Task>? OnOpenedAsync;
        public event Func<string, Task>? OnTitleUpdatedAsync;
        public event Func<Task>? OnDeletedAsync;

        public SignalrQuizSyncContext(ILogger<SignalrQuizSyncContext> logger, HubConnection hubConnection)
            : base(logger, hubConnection)
        {
            Bind(e => e.OnQuizOpenedAsync, () => OnOpenedAsync);
            Bind(e => e.OnQuizTitleUpdatedAsync, () => OnTitleUpdatedAsync);
            Bind(e => e.OnQuizDeletedAsync, () => OnDeletedAsync);
        }

        public Task OpenAsync() =>
            HubConnection.InvokeAsync(SignalrEndpoints.OpenQuiz);

        public Task UpdateTitleAsync(string newTitle) =>
            HubConnection.InvokeAsync(SignalrEndpoints.UpdateQuizTitle, newTitle);

        public Task DeleteAsync() =>
            HubConnection.InvokeAsync(SignalrEndpoints.DeleteQuiz);
    }
}
