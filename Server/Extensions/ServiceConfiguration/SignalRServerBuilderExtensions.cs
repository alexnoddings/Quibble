using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace Quibble.Server.Extensions.ServiceConfiguration
{
    /// <summary>
    /// Extension methods for setting up SignalR services in an <see cref="ISignalRServerBuilder" />.
    /// </summary>
    public static class SignalRServerBuilderExtensions
    {
        /// <inheritdoc cref="AddJwtBearerAuthenticationCore"/>
        public static ISignalRServerBuilder AddJwtBearerAuthentication(this ISignalRServerBuilder signalrBuilder)
        {
            signalrBuilder.AddJwtBearerAuthenticationCore("/hubs", StringComparison.OrdinalIgnoreCase);
            return signalrBuilder;
        }

        /// <inheritdoc cref="AddJwtBearerAuthenticationCore"/>
        public static ISignalRServerBuilder AddJwtBearerAuthentication(this ISignalRServerBuilder signalrBuilder, string hubPathStartSegment, StringComparison hubPathComparison = StringComparison.OrdinalIgnoreCase)
        {
            signalrBuilder.AddJwtBearerAuthenticationCore(hubPathStartSegment, hubPathComparison);
            return signalrBuilder;
        }

        /// <summary>
        /// Adds JWT Bearer authentication to an  <see cref="ISignalRServerBuilder"/>.
        /// </summary>
        /// <param name="signalrBuilder">The <see cref="ISignalRServerBuilder"/>.</param>
        /// <param name="hubPathStartSegment">The starting segment to check if a request is for a SignalR hub.</param>
        /// <param name="hubPathComparison">The comparison method to check if a request if for a SignalR hub.</param>
        /// <returns>The same instance of the <see cref="ISignalRServerBuilder"/> for chaining.</returns>
        private static ISignalRServerBuilder AddJwtBearerAuthenticationCore(this ISignalRServerBuilder signalrBuilder, string hubPathStartSegment, StringComparison hubPathComparison = StringComparison.OrdinalIgnoreCase)
        {
            if (signalrBuilder == null) throw new ArgumentNullException(nameof(signalrBuilder));

            // SignalR's websockets don't support normal authentication. As such, the access token is passed in the query and picked up by the JWT bearer.
            signalrBuilder.Services.PostConfigureAll<JwtBearerOptions>(options =>
            {
                var originalOnMessageReceived = options.Events.OnMessageReceived;
                options.Events.OnMessageReceived = async context =>
                {
                    // Ensure the original event handler is executed
                    await originalOnMessageReceived(context).ConfigureAwait(false);
                    PathString path = context.HttpContext.Request.Path;
                    // Only run when the request is directed to a SignalR hub
                    if (path.StartsWithSegments(hubPathStartSegment, hubPathComparison))
                    {
                        if (string.IsNullOrEmpty(context.Token))
                        {
                            if (context.Request.Query.TryGetValue("access_token", out StringValues accessToken) && !string.IsNullOrEmpty(accessToken))
                                context.Token = accessToken;
                        }
                    }
                };
            });

            return signalrBuilder;
        }

    }
}
