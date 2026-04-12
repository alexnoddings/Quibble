using System.Text.Json;
using System.Text.Json.Serialization;
using Quibble.Answers;
using Quibble.Api;
using Quibble.Api.Context;
using Quibble.Api.Validation;
using Quibble.Games;
using Quibble.Games.Events;
using Quibble.Questions;
using Quibble.Rounds;
using Quibble.Users;

namespace Quibble.Hosting;

public static class ApiExtensions
{
    public static TBuilder AddQuibbleApi<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddOpenApi();
        builder.Services.AddCors();

        builder.Services.AddProblemDetails();

        builder.Services.AddResponseCompression();

        builder.Services.AddServiceDiscovery();
        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            http.AddStandardResilienceHandler();
            http.AddServiceDiscovery();
        });

        builder.Services.AddResponseCompression();

        builder.Services.ConfigureHttpJsonOptions(options =>
            ConfigureJsonSerialisation(options.SerializerOptions)
        );

        builder.Services
            .AddSignalR(o =>
            {
                o.EnableDetailedErrors = builder.Environment.IsDevelopment();
            })
            .AddJsonProtocol(o =>
                ConfigureJsonSerialisation(o.PayloadSerializerOptions)
            );

        return builder;
    }

    private static void ConfigureJsonSerialisation(JsonSerializerOptions options)
    {
        options.Converters.Add(new JsonStringEnumConverter());
    }

    public static RouteGroupBuilder MapQuibbleApi(this IEndpointRouteBuilder endpoints)
    {
        var api = endpoints
            .MapGroup("/api")
            .RequireAuthorization();

        api.MapGroup("/users")
            .MapUsersApi();

        api.MapGroup("")
            .InjectContext()
            .AddRequirementFilters()
            .AddValidationFilters()
            .MapGamesApi()
            .MapRoundsApi()
            .MapQuestionsApi()
            .MapAnswersApi()
            .MapGameEvents();

        return api;
    }
}
