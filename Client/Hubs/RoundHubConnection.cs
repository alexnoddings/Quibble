using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using Quibble.Common;
using Quibble.Common.Rounds;

namespace Quibble.Client.Hubs
{
    public class RoundHubConnection : BaseSecureHubConnection, IRoundHub
    {
        public RoundHubConnection(NavigationManager navigationManager, IAccessTokenProvider accessTokenProvider) 
            : base(navigationManager, accessTokenProvider, SignalRPaths.RoundHub)
        {
        }

        public Task<Round> CreateAsync(Round round) =>
            HubConnection.InvokeAsync<Round>(nameof(IRoundHub.CreateAsync), round);

        public Task<Round> GetAsync(Guid id) =>
            HubConnection.InvokeAsync<Round>(nameof(IRoundHub.GetAsync), id);

        public Task<RoundFull> GetFullAsync(Guid id) =>
            HubConnection.InvokeAsync<RoundFull>(nameof(IRoundHub.GetFullAsync), id);

        public Task<Round> UpdateAsync(Round round) =>
            HubConnection.InvokeAsync<Round>(nameof(IRoundHub.UpdateAsync), round);

        public Task DeleteAsync(Guid id) =>
            HubConnection.InvokeAsync(nameof(IRoundHub.DeleteAsync), id);

        public Task RegisterForUpdatesAsync(Guid quizId) =>
            HubConnection.InvokeAsync(nameof(IRoundHub.RegisterForUpdatesAsync), quizId);

        public IDisposable OnRoundCreated(Func<Round, Task> handler) =>
            HubConnection.On(nameof(IRoundHubClient.OnRoundCreated), handler);

        public IDisposable OnRoundUpdated(Func<Round, Task> handler) =>
            HubConnection.On(nameof(IRoundHubClient.OnRoundUpdated), handler);

        public IDisposable OnRoundDeleted(Func<Guid, Task> handler) =>
            HubConnection.On(nameof(IRoundHubClient.OnRoundDeleted), handler);

    }
}
