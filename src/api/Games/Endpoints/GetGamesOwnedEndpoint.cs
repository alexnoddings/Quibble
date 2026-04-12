using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quibble.Api;
using Quibble.Api.Users;
using Quibble.Data;

namespace Quibble.Games.Endpoints;

public static class GetGamesOwnedEndpoint
{
    public static IEndpointRouteBuilder MapGetOwnedGames(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"/{Routes.GamesBase}/owned", GetGamesOwnedAsync);
        return endpoints;
    }

    private static async Task<object?> GetGamesOwnedAsync(
        [FromServices] IDbContextFactory<QuibbleDbContext> dbContextFactory,
        UserContext userContext
    )
    {
        var user = userContext.User;

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var games =
            await dbContext.Games
            .Where(g => g.OwnerId == user.Id)
            .Select(g => new
            {
                g.Id,
                g.Slug,
                g.Title,
                g.State
            })
            .ToListAsync();

        return games;
    }
}
