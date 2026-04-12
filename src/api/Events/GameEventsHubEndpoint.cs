using Quibble.Api;

namespace Quibble.Games.Events;

public static class GameEventsHubEndpoint
{
    public static IEndpointRouteBuilder MapGameEvents(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHub<GameEventsHub>($"{Routes.GamesBase}/{Routes.GameId}/events");
        return endpoints;
    }
}
