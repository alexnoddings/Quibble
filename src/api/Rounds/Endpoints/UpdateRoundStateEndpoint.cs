using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quibble.Api;
using Quibble.Api.Games;
using Quibble.Api.Rounds;
using Quibble.Api.Users;
using Quibble.Api.Validation;
using Quibble.Data;
using Quibble.Data.Entites.Games;
using Quibble.Data.Entites.Rounds;

namespace Quibble.Rounds.Endpoints;

public class UpdateRoundStateRequest
{
    public RoundState State { get; set; }
}

public class UpdateRoundStateRequestValidator : AbstractValidator<UpdateRoundStateRequest>
{
    public UpdateRoundStateRequestValidator()
    {
    }
}

public static class UpdateRoundEndpoint
{
    public static IEndpointRouteBuilder MapUpdateRoundState(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut($"/{Routes.RoundsBase}/{Routes.RoundId}/state", UpdateRoundStateAsync);
        return endpoints;
    }

    [RequireGame, RequireRound]
    [RequireUserOwnsGame]
    [RequireGameState(GameState.InProgress)]
    private static async Task UpdateRoundStateAsync(
        [FromServices] IDbContextFactory<QuibbleDbContext> dbContextFactory,
        [FromBody, Validate] UpdateRoundStateRequest request,
        RoundContext roundContext,
        GameEvents gameEvents,
        HttpContext httpContext
    )
    {
        var round = roundContext.Round;

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var dbRound = await dbContext.Rounds.FirstAsync(r => r.Id == round.Id);

        // Can only transition from Hidden to Visible
        if (request.State is not RoundState.Visible)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        // Already visible
        if (dbRound.State is RoundState.Visible)
            return;

        dbRound.State = RoundState.Visible;

        await dbContext.SaveChangesAsync();

        await gameEvents
            .SendToEveryone()
            .Round(round)
            .StateChangedAsync(dbRound.State);
    }
}
