using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using Quibble.Common;
using Quibble.Common.Quizzes;

namespace Quibble.Client.Hubs
{
    public class QuizHubConnection : BaseSecureHubConnection, IQuizHub
    {
        public QuizHubConnection(NavigationManager navigationManager, IAccessTokenProvider accessTokenProvider) 
            : base(navigationManager, accessTokenProvider, SignalRPaths.QuizHub)
        {
        }

        public Task<Quiz> CreateAsync(Quiz quiz) =>
            HubConnection.InvokeAsync<Quiz>(nameof(IQuizHub.CreateAsync), quiz);

        public Task<Quiz> GetAsync(Guid id) =>
            HubConnection.InvokeAsync<Quiz>(nameof(IQuizHub.GetAsync), id);

        public Task<List<Quiz>> GetOwnedAsync() =>
            HubConnection.InvokeAsync<List<Quiz>>(nameof(IQuizHub.GetOwnedAsync));

        public Task<QuizFull> GetFullAsync(Guid id) =>
            HubConnection.InvokeAsync<QuizFull>(nameof(IQuizHub.GetFullAsync), id);

        public Task<Quiz> UpdateAsync(Quiz quiz) =>
            HubConnection.InvokeAsync<Quiz>(nameof(IQuizHub.UpdateAsync), quiz);

        public Task DeleteAsync(Guid id) =>
            HubConnection.InvokeAsync(nameof(IQuizHub.DeleteAsync), id);

        public Task RegisterForUpdatesAsync(Guid quizId) =>
            HubConnection.InvokeAsync(nameof(IQuizHub.RegisterForUpdatesAsync), quizId);

        public IDisposable OnQuizUpdated(Func<Quiz, Task> handler) =>
            HubConnection.On(nameof(IQuizHubClient.OnQuizUpdated), handler);

        public IDisposable OnQuizDeleted(Func<Guid, Task> handler) =>
            HubConnection.On(nameof(IQuizHubClient.OnQuizDeleted), handler);
    }
}
