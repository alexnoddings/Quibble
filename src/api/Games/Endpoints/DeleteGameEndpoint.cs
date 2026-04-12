using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quibble.Api;
using Quibble.Api.Games;
using Quibble.Api.Users;
using Quibble.Data;
using Quibble.Data.Entites.Games;
using Quibble.Games.Info;

namespace Quibble.Games.Endpoints;

public static class DeleteGameEndpoint
{
    public static IEndpointRouteBuilder MapDeleteGame(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete($"/{Routes.GamesBase}/{Routes.GameId}", DeleteGameAsync);
        return endpoints;
    }

    [RequireGame]
    [RequireUserOwnsGame]
    [RequireGameState(GameState.Draft)]
    private static async Task DeleteGameAsync(
        [FromServices] IGameInfoService gameService,
        [FromServices] IDbContextFactory<QuibbleDbContext> dbContextFactory,
        GameContext gameContext,
        HttpContext httpContext
    )
    {
        var game = gameContext.Game;

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        dbContext.Games.Add(new Game { Id = game.Id }).State = EntityState.Deleted;

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) { }

        await gameService.OnGameDeletedAsync(game.Id);
    }
}
