using Quibble.Games.Endpoints;

namespace Quibble.Games;

public static class GamesEndpoints
{
    public static IEndpointRouteBuilder MapGamesApi(this IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapGetGame()
            .MapGetGameInfo()
            .MapGetOwnedGames()
            .MapGetParticipatingGames()
            .MapCreateGame()
            .MapJoinGame()
            .MapUpdateGameTitle()
            .MapUpdateGameState()
            .MapDeleteGame();

        return endpoints;
    }
}
