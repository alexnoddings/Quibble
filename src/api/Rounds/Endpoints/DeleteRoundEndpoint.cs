using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quibble.Api;
using Quibble.Api.Games;
using Quibble.Api.Rounds;
using Quibble.Api.Users;
using Quibble.Data;
using Quibble.Data.Entites.Games;
using Quibble.Data.Entites.Rounds;

namespace Quibble.Rounds.Endpoints;

public static class DeleteRoundEndpoint
{
    public static IEndpointRouteBuilder MapDeleteRound(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete($"/{Routes.RoundsBase}/{Routes.RoundId}", DeleteRoundAsync);
        return endpoints;
    }

    [RequireGame, RequireRound]
    [RequireUserOwnsGame]
    [RequireGameState(GameState.Draft)]
    private static async Task DeleteRoundAsync(
        [FromServices] IDbContextFactory<QuibbleDbContext> dbContextFactory,
        RoundContext roundContext,
        GameEvents gameEvents,
        HttpContext httpContext
    )
    {
        var round = roundContext.Round;

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var roundExists = await dbContext.Rounds.AnyAsync(r => r.Id == round.Id);
        if (!roundExists)
        {
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        dbContext.Rounds.Add(new Round { Id = round.Id }).State = EntityState.Deleted;

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return;
        }

        await gameEvents
            .SendToEveryone()
            .Round(round)
            .RemovedAsync();
    }
}
