using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Quibble.Client.Extensions.SignalR
{
    public static class ServiceCollectionExtensions
    {
        [SuppressMessage("Design", "CA1054:Uri parameters should not be strings", Justification = "It is a string to allow for a relative url to be passed. Transformation to an absolute Uri should not be handled by the caller.")]
        public static IServiceCollection AddHubConnection<THubConnection>(this IServiceCollection serviceCollection, string hubRelativeUrl, Func<HubConnection, THubConnection> implementationFactory)
            where THubConnection : class
        {
            serviceCollection.AddScoped<THubConnection>(serviceProvider =>
            {
                var innerHubConnection = BuildHubConnection(serviceProvider, hubRelativeUrl);
                return implementationFactory(innerHubConnection);
            });

            return serviceCollection;
        }

        private static HubConnection BuildHubConnection(IServiceProvider serviceProvider, string relativeUrl)
        {
            var navigationManager = serviceProvider.GetRequiredService<NavigationManager>();
            var accessTokenProvider = serviceProvider.GetRequiredService<IAccessTokenProvider>();

            return new HubConnectionBuilder()
                    .WithAuthenticatedRelativeUrl(navigationManager, relativeUrl, accessTokenProvider)
                    .Build();
        }
    }
}
