using Microsoft.AspNetCore.Mvc;
using Quibble.Api;
using Quibble.Api.Users;
using Quibble.Data.Entites.Games;
using Quibble.Games.Info;

namespace Quibble.Games.Endpoints;

public enum GameRelationship
{
    Owner,
    Participant,
    None
}

public class GameInfoResponse
{
    public Guid Id { get; set; }
    public GameState State { get; set; }
    public GameRelationship Relationship { get; set; }
}

public static class GetGameInfoEndpoint
{
    public static IEndpointRouteBuilder MapGetGameInfo(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"/{Routes.GamesBase}/info/{{GameSlug}}", GetGameInfoAsync);
        return endpoints;
    }

    private static async Task<GameInfoResponse?> GetGameInfoAsync(
        [FromServices] IGameInfoService gameInfoService,
        [FromRoute] string gameSlug,
        UserContext userContext,
        HttpContext httpContext
    )
    {
        var user = userContext.User;

        var game = await gameInfoService.GetGameBySlugAsync(gameSlug);
        if (game is null)
        {
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            return null;
        }

        if (game.OwnerId != user.Id && game.State is GameState.Draft)
        {
            httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            return null;
        }

        GameRelationship relationship;
        if (game.OwnerId == user.Id)
            relationship = GameRelationship.Owner;
        else if (game.Participants.Any(p => p.UserId == user.Id))
            relationship = GameRelationship.Participant;
        else
            relationship = GameRelationship.None;

        return new GameInfoResponse
        {
            Id = game.Id,
            State = game.State,
            Relationship = relationship
        };
    }
}
