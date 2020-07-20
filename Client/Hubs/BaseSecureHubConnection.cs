using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using Quibble.Client.Extensions.SignalR;

namespace Quibble.Client.Hubs
{
    public class BaseSecureHubConnection : IAsyncDisposable
    {
        protected HubConnection HubConnection { get; }

        [SuppressMessage("Design", "CA1054:Uri parameters should not be strings", Justification = SuppressionJustifications.CA1054)]
        public BaseSecureHubConnection(NavigationManager navigationManager, IAccessTokenProvider accessTokenProvider, string relativeHubUrl)
        {
            HubConnection = new HubConnectionBuilder()
                .WithAuthenticatedRelativeUrl(navigationManager, relativeHubUrl, accessTokenProvider)
                .Build();
        }

        public Task StartAsync() =>
            HubConnection.StartAsync();

        public async ValueTask DisposeAsync()
        {
            await HubConnection.DisposeAsync().ConfigureAwait(false);
        }
    }
}
