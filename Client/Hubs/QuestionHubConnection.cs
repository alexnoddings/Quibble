using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using Quibble.Common;
using Quibble.Common.Questions;

namespace Quibble.Client.Hubs
{
    public class QuestionHubConnection : BaseSecureHubConnection, IQuestionHub
    {
        public QuestionHubConnection(NavigationManager navigationManager, IAccessTokenProvider accessTokenProvider) 
            : base(navigationManager, accessTokenProvider, SignalRPaths.QuestionHub)
        {
        }

        public Task<Question> CreateAsync(Question question) =>
            HubConnection.InvokeAsync<Question>(nameof(IQuestionHub.CreateAsync), question);

        public Task<Question> GetAsync(Guid id) =>
            HubConnection.InvokeAsync<Question>(nameof(IQuestionHub.GetAsync), id);

        public Task<Question> UpdateAsync(Question question) =>
            HubConnection.InvokeAsync<Question>(nameof(IQuestionHub.UpdateAsync), question);

        public Task DeleteAsync(Guid id) =>
            HubConnection.InvokeAsync(nameof(IQuestionHub.DeleteAsync), id);

        public Task RegisterForUpdatesAsync(Guid quizId) =>
            HubConnection.InvokeAsync(nameof(IQuestionHub.RegisterForUpdatesAsync), quizId);

        public IDisposable OnQuestionCreated(Func<Question, Task> handler) =>
            HubConnection.On(nameof(IQuestionHubClient.OnQuestionCreated), handler);

        public IDisposable OnQuestionUpdated(Func<Question, Task> handler) =>
            HubConnection.On(nameof(IQuestionHubClient.OnQuestionUpdated), handler);

        public IDisposable OnQuestionDeleted(Func<Guid, Task> handler) =>
            HubConnection.On(nameof(IQuestionHubClient.OnQuestionDeleted), handler);

    }
}
