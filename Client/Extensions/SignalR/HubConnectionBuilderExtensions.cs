using System;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Quibble.Client.Handlers;

namespace Quibble.Client.Extensions.SignalR
{
    /// <summary>
    /// Extension methods for <see cref="IHubConnectionBuilder"/>.
    /// </summary>
    public static class HubConnectionBuilderExtensions
    {
        /// <inheritdoc cref="WithAuthenticatedUrlCore"/>
        public static IHubConnectionBuilder WithAuthenticatedUrl(this IHubConnectionBuilder hubConnectionBuilder, Uri url, IAccessTokenProvider accessTokenProvider)
        {
            hubConnectionBuilder.WithAuthenticatedUrlCore(url, accessTokenProvider, null);
            return hubConnectionBuilder;
        }

        /// <inheritdoc cref="WithAuthenticatedUrlCore"/>
        public static IHubConnectionBuilder WithAuthenticatedUrl(this IHubConnectionBuilder hubConnectionBuilder, Uri url, IAccessTokenProvider accessTokenProvider, Action<HttpConnectionOptions>? configureHttpConnection)
        {
            hubConnectionBuilder.WithAuthenticatedUrlCore(url, accessTokenProvider, configureHttpConnection);
            return hubConnectionBuilder;
        }

        /// <inheritdoc cref="WithAuthenticatedUrlCore"/>
        public static IHubConnectionBuilder WithAuthenticatedUrl(this IHubConnectionBuilder hubConnectionBuilder, string url, IAccessTokenProvider accessTokenProvider)
        {
            hubConnectionBuilder.WithAuthenticatedUrlCore(new Uri(url), accessTokenProvider, null);
            return hubConnectionBuilder;
        }

        /// <inheritdoc cref="WithAuthenticatedUrlCore"/>
        public static IHubConnectionBuilder WithAuthenticatedUrl(this IHubConnectionBuilder hubConnectionBuilder, string url, IAccessTokenProvider accessTokenProvider, Action<HttpConnectionOptions>? configureHttpConnection)
        {
            hubConnectionBuilder.WithAuthenticatedUrlCore(new Uri(url), accessTokenProvider, configureHttpConnection);
            return hubConnectionBuilder;
        }

        /// <summary>
        /// Configures the <see cref="HubConnection"/> to use access token authenticated HTTP-based transports to connect to the specified URL.
        /// </summary>
        /// <param name="hubConnectionBuilder">The <see cref="IHubConnectionBuilder" /> to configure.</param>
        /// <param name="url">The URI the <see cref="HttpConnection"/> will use.</param>
        /// <param name="accessTokenProvider">A service which provides an <see cref="AccessToken"/>.</param>
        /// <param name="configureHttpConnection">The delegate that configures the <see cref="HttpConnection"/>.</param>
        /// <returns>The same instance of the <see cref="IHubConnectionBuilder"/> for chaining.</returns>
        private static IHubConnectionBuilder WithAuthenticatedUrlCore(this IHubConnectionBuilder hubConnectionBuilder, Uri url, IAccessTokenProvider accessTokenProvider, Action<HttpConnectionOptions>? configureHttpConnection)
        {
            if (hubConnectionBuilder == null) throw new ArgumentNullException(nameof(hubConnectionBuilder));
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (accessTokenProvider == null) throw new ArgumentNullException(nameof(accessTokenProvider));

            return hubConnectionBuilder.WithUrl(url, options =>
            {
                ConfigureAuthenticationForHttpConnection(options, accessTokenProvider);
                configureHttpConnection?.Invoke(options);
            });
        }

        /// <summary>
        /// Configures the <see cref="HttpConnection"/> to attach an access token to same-origin requests.
        /// </summary>
        /// <param name="options">The options instance.</param>
        /// <param name="accessTokenProvider">A service which provides an <see cref="AccessToken"/>.</param>
        private static void ConfigureAuthenticationForHttpConnection(HttpConnectionOptions options, IAccessTokenProvider accessTokenProvider)
        {
            options.HttpMessageHandlerFactory = originalHandler => new SameOriginRequestCredentialsHttpMessageHandler(originalHandler);
            options.AccessTokenProvider = async () =>
            {
                AccessTokenResult result = await accessTokenProvider.RequestAccessToken().ConfigureAwait(false);
                AccessToken? token = null;
                result?.TryGetToken(out token);
                return token?.Value;
            };
        }
    }
}
