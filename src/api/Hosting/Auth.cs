using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;
using Quibble.Users;

namespace Quibble.Hosting;

public static class AuthExtensions
{
    // private static bool ShouldUseMockAuth(IHostApplicationBuilder builder) =>
    //     builder.Environment.Is(Env.Development, Env.AutomatedTesting);

    public static TBuilder AddQuibbleAuthentication<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        var auth = builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);

        var config = builder.Configuration.GetSection("AzureAd");
        auth.AddMicrosoftIdentityWebApi(
            configureMicrosoftIdentityOptions: config.Bind,
            configureJwtBearerOptions: options =>
            {
                config.Bind(options);

                // The SignalR browser client sends JWT access tokens via the
                // query string for WebSocket and Server-Sent Events.
                // We hook into messages being received to extract
                // the query string's access token for SignalR connections.
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = SetTokenFromQueryString
                };
            });

        if (builder.Environment.Is(Env.Development))
        {
            IdentityModelEventSource.LogCompleteSecurityArtifact = true;
            IdentityModelEventSource.ShowPII = true;
        }

        // if (ShouldUseMockAuth(builder))
        // {
        //     auth.AddScheme<MockAuthenticationSchemeOptions, MockAuthenticationHandler>(
        //         MockAuthDefaults.AuthenticationScheme,
        //         configureOptions: _ => { }
        //     );
        // }

        return builder;
    }

    private static Task SetTokenFromQueryString(MessageReceivedContext context)
    {
        // SignalR websockets open by sending a CONNECT request to the server
        var isConnect = context.Request.Method == HttpMethods.Connect;
        if (!isConnect)
            return Task.CompletedTask;

        // And the endpoint should be decorated with HubMetadata by SignalR
        var isHubEndpoint = context.HttpContext.GetEndpoint()?.Metadata.GetMetadata<HubMetadata>() != null;
        if (!isHubEndpoint)
            return Task.CompletedTask;

        var accessToken = (string?)context.Request.Query["access_token"];
        if (!string.IsNullOrEmpty(accessToken))
            context.Token = accessToken;

        return Task.CompletedTask;
    }

    public static TBuilder AddQuibbleAuthorization<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        var authSchemes = new List<string> { JwtBearerDefaults.AuthenticationScheme };

        // if (ShouldUseMockAuth(builder))
        //     authSchemes.Add(MockAuthDefaults.AuthenticationScheme);

        builder.Services
            .AddAuthorizationBuilder()
            .SetDefaultPolicy(
                new AuthorizationPolicyBuilder([..authSchemes])
                    .RequireAuthenticatedUser()
                    .RequireValidUser()
                    .Build()
            );

        builder.Services.AddScoped<IAuthorizationHandler, ValidUserAuthorizationHandler>();

        return builder;
    }
}
