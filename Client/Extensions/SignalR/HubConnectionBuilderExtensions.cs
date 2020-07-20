using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Quibble.Client.Handlers;

namespace Quibble.Client.Extensions.SignalR
{
    /// <summary>
    /// Extension methods for <see cref="IHubConnectionBuilder"/>.
    /// </summary>
    [SuppressMessage("Design", "CA1054:Uri parameters should not be strings", Justification = SuppressionJustifications.CA1054)]
    public static class HubConnectionBuilderExtensions
    {
        /// <summary>
        /// Configures the <see cref="HubConnection"/> to use access token authenticated HTTP-based transports to connect to the specified URL.
        /// </summary>
        /// <param name="hubConnectionBuilder">The <see cref="IHubConnectionBuilder" /> to configure.</param>
        /// <param name="url">The URL the <see cref="HttpConnection"/> will use.</param>
        /// <param name="accessTokenProvider">A service which provides an <see cref="AccessToken"/>.</param>
        /// <returns>The same instance of the <see cref="IHubConnectionBuilder"/> for chaining.</returns>
        /// <remarks>See <see cref="HubConnectionBuilderHttpExtensions.WithUrl(IHubConnectionBuilder, Uri, Action{HttpConnectionOptions})"/></remarks>
        public static IHubConnectionBuilder WithAuthenticatedUrl(this IHubConnectionBuilder hubConnectionBuilder, Uri url, IAccessTokenProvider accessTokenProvider)
        {
            hubConnectionBuilder.WithAuthenticatedUrlCore(url, accessTokenProvider, null);
            return hubConnectionBuilder;
        }

        /// <summary>
        /// Configures the <see cref="HubConnection"/> to use access token authenticated HTTP-based transports to connect to the specified URL.
        /// </summary>
        /// <param name="hubConnectionBuilder">The <see cref="IHubConnectionBuilder" /> to configure.</param>
        /// <param name="url">The URL the <see cref="HttpConnection"/> will use.</param>
        /// <param name="accessTokenProvider">A service which provides an <see cref="AccessToken"/>.</param>
        /// <param name="configureHttpConnection">The delegate that configures the <see cref="HttpConnection"/>.</param>
        /// <returns>The same instance of the <see cref="IHubConnectionBuilder"/> for chaining.</returns>
        /// <remarks>See <see cref="HubConnectionBuilderHttpExtensions.WithUrl(IHubConnectionBuilder, Uri, Action{HttpConnectionOptions})"/></remarks>
        public static IHubConnectionBuilder WithAuthenticatedUrl(this IHubConnectionBuilder hubConnectionBuilder, Uri url, IAccessTokenProvider accessTokenProvider, Action<HttpConnectionOptions>? configureHttpConnection)
        {
            hubConnectionBuilder.WithAuthenticatedUrlCore(url, accessTokenProvider, configureHttpConnection);
            return hubConnectionBuilder;
        }

        /// <summary>
        /// Configures the <see cref="HubConnection"/> to use access token authenticated HTTP-based transports to connect to the specified URL.
        /// </summary>
        /// <param name="hubConnectionBuilder">The <see cref="IHubConnectionBuilder" /> to configure.</param>
        /// <param name="absoluteUrl">The absolute URL the <see cref="HttpConnection"/> will use. For relative URLs, see <see cref="WithAuthenticatedRelativeUrl(IHubConnectionBuilder,NavigationManager,string,IAccessTokenProvider)"/>.</param>
        /// <param name="accessTokenProvider">A service which provides an <see cref="AccessToken"/>.</param>
        /// <returns>The same instance of the <see cref="IHubConnectionBuilder"/> for chaining.</returns>
        /// <remarks>See <see cref="HubConnectionBuilderHttpExtensions.WithUrl(IHubConnectionBuilder, Uri, Action{HttpConnectionOptions})"/></remarks>
        public static IHubConnectionBuilder WithAuthenticatedAbsoluteUrl(this IHubConnectionBuilder hubConnectionBuilder, string absoluteUrl, IAccessTokenProvider accessTokenProvider)
        {
            if (absoluteUrl == null) throw new ArgumentNullException(nameof(absoluteUrl));

            var absoluteUrlAsUri = new Uri(absoluteUrl);
            if (absoluteUrlAsUri.Scheme != "http" && absoluteUrlAsUri.Scheme != "https")
                throw new ArgumentException("Only http and https schemes are supported.");

            hubConnectionBuilder.WithAuthenticatedUrlCore(absoluteUrlAsUri, accessTokenProvider, null);
            return hubConnectionBuilder;
        }

        /// <summary>
        /// Configures the <see cref="HubConnection"/> to use access token authenticated HTTP-based transports to connect to the specified URL.
        /// </summary>
        /// <param name="hubConnectionBuilder">The <see cref="IHubConnectionBuilder" /> to configure.</param>
        /// <param name="absoluteUrl">The absolute URL the <see cref="HttpConnection"/> will use. For relative URLs, see <see cref="WithAuthenticatedRelativeUrl(IHubConnectionBuilder,NavigationManager,string,IAccessTokenProvider,Action{HttpConnectionOptions})"/>.</param>
        /// <param name="accessTokenProvider">A service which provides an <see cref="AccessToken"/>.</param>
        /// <param name="configureHttpConnection">The delegate that configures the <see cref="HttpConnection"/>.</param>
        /// <returns>The same instance of the <see cref="IHubConnectionBuilder"/> for chaining.</returns>
        /// <remarks>See <see cref="HubConnectionBuilderHttpExtensions.WithUrl(IHubConnectionBuilder, Uri, Action{HttpConnectionOptions})"/></remarks>
        public static IHubConnectionBuilder WithAuthenticatedAbsoluteUrl(this IHubConnectionBuilder hubConnectionBuilder, string absoluteUrl, IAccessTokenProvider accessTokenProvider, Action<HttpConnectionOptions>? configureHttpConnection)
        {
            if (absoluteUrl == null) throw new ArgumentNullException(nameof(absoluteUrl));

            var absoluteUrlAsUri = new Uri(absoluteUrl);
            if (absoluteUrlAsUri.Scheme != "http" && absoluteUrlAsUri.Scheme != "https")
                throw new ArgumentException("Only http and https schemes are supported.");

            hubConnectionBuilder.WithAuthenticatedUrlCore(absoluteUrlAsUri, accessTokenProvider, configureHttpConnection);
            return hubConnectionBuilder;
        }

        /// <summary>
        /// Configures the <see cref="HubConnection"/> to use access token authenticated HTTP-based transports to connect to the specified URL.
        /// </summary>
        /// <param name="hubConnectionBuilder">The <see cref="IHubConnectionBuilder" /> to configure.</param>
        /// <param name="navigationManager">A <see cref="NavigationManager"/> to find the absolute url.</param>
        /// <param name="relativeUrl">A relative URL the <see cref="HttpConnection"/> will use. This is made absolute by the <paramref name="navigationManager"/>. For absolute URLs, see <see cref="WithAuthenticatedAbsoluteUrl(IHubConnectionBuilder,string,IAccessTokenProvider)"/></param>
        /// <param name="accessTokenProvider">A service which provides an <see cref="AccessToken"/>.</param>
        /// <returns>The same instance of the <see cref="IHubConnectionBuilder"/> for chaining.</returns>
        /// <remarks>See <see cref="HubConnectionBuilderHttpExtensions.WithUrl(IHubConnectionBuilder, Uri, Action{HttpConnectionOptions})"/></remarks>
        public static IHubConnectionBuilder WithAuthenticatedRelativeUrl(this IHubConnectionBuilder hubConnectionBuilder, NavigationManager navigationManager, string relativeUrl, IAccessTokenProvider accessTokenProvider)
        {
            if (relativeUrl == null) throw new ArgumentNullException(nameof(relativeUrl));
            if (navigationManager == null) throw new ArgumentNullException(nameof(navigationManager));

            Uri absoluteUrl = navigationManager.ToAbsoluteUri(relativeUrl);

            hubConnectionBuilder.WithAuthenticatedUrlCore(absoluteUrl, accessTokenProvider, null);
            return hubConnectionBuilder;
        }


        /// <summary>
        /// Configures the <see cref="HubConnection"/> to use access token authenticated HTTP-based transports to connect to the specified URL.
        /// </summary>
        /// <param name="hubConnectionBuilder">The <see cref="IHubConnectionBuilder" /> to configure.</param>
        /// <param name="navigationManager">A <see cref="NavigationManager"/> to find the absolute url.</param>
        /// <param name="relativeUrl">A relative URL the <see cref="HttpConnection"/> will use. This is made absolute by the <paramref name="navigationManager"/>. For absolute URLs, see <see cref="WithAuthenticatedAbsoluteUrl(IHubConnectionBuilder,string,IAccessTokenProvider,Action{HttpConnectionOptions})"/></param>
        /// <param name="accessTokenProvider">A service which provides an <see cref="AccessToken"/>.</param>
        /// <param name="configureHttpConnection">The delegate that configures the <see cref="HttpConnection"/>.</param>
        /// <returns>The same instance of the <see cref="IHubConnectionBuilder"/> for chaining.</returns>
        /// <remarks>See <see cref="HubConnectionBuilderHttpExtensions.WithUrl(IHubConnectionBuilder, Uri, Action{HttpConnectionOptions})"/></remarks>
        public static IHubConnectionBuilder WithAuthenticatedRelativeUrl(this IHubConnectionBuilder hubConnectionBuilder, NavigationManager navigationManager, string relativeUrl, IAccessTokenProvider accessTokenProvider, Action<HttpConnectionOptions>? configureHttpConnection)
        {
            if (relativeUrl == null) throw new ArgumentNullException(nameof(relativeUrl));
            if (navigationManager == null) throw new ArgumentNullException(nameof(navigationManager));

            Uri absoluteUrl = navigationManager.ToAbsoluteUri(relativeUrl);

            hubConnectionBuilder.WithAuthenticatedUrlCore(absoluteUrl, accessTokenProvider, configureHttpConnection);
            return hubConnectionBuilder;
        }

        /// <summary>
        /// Configures the <see cref="HubConnection"/> to use access token authenticated HTTP-based transports to connect to the specified URL.
        /// </summary>
        /// <param name="hubConnectionBuilder">The <see cref="IHubConnectionBuilder" /> to configure.</param>
        /// <param name="url">The URL the <see cref="HttpConnection"/> will use.</param>
        /// <param name="accessTokenProvider">A service which provides an <see cref="AccessToken"/>.</param>
        /// <param name="configureHttpConnection">The delegate that configures the <see cref="HttpConnection"/>.</param>
        /// <returns>The same instance of the <see cref="IHubConnectionBuilder"/> for chaining.</returns>
        /// <remarks>See <see cref="HubConnectionBuilderHttpExtensions.WithUrl(IHubConnectionBuilder, Uri, Action{HttpConnectionOptions})"/></remarks>
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
