using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quibble.Api;
using Quibble.Api.Users;
using Quibble.Data;

namespace Quibble.Games.Endpoints;

public static class GetGamesParticipatingEndpoint
{
    public static IEndpointRouteBuilder MapGetParticipatingGames(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"/{Routes.GamesBase}/participating", GetGamesParticipatingAsync);
        return endpoints;
    }

    private static async Task<object?> GetGamesParticipatingAsync(
        [FromServices] IDbContextFactory<QuibbleDbContext> dbContextFactory,
        UserContext userContext
    )
    {
        var user = userContext.User;

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var games =
            await dbContext.Games
            .Where(g => g.Participants.Any(p => p.UserId == user.Id))
            .Select(g => new
            {
                g.Id,
                g.Slug,
                OwnerName = g.Owner.DisplayName,
                g.Title,
                g.State
            })
            .ToListAsync();

        return games;
    }
}
