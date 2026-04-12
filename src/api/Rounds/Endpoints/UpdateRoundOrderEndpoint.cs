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

namespace Quibble.Rounds.Endpoints;

public class UpdateRoundOrderRequest
{
    public int Order { get; set; }
}

public class UpdateRoundOrderRequestValidator : AbstractValidator<UpdateRoundOrderRequest>
{
    public UpdateRoundOrderRequestValidator()
    {
        RuleFor(x => x.Order)
            .NotNull()
            .GreaterThanOrEqualTo(0);
    }
}

public static class UpdateRoundOrderEndpoint
{
    public static IEndpointRouteBuilder MapUpdateRoundOrder(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut($"/{Routes.RoundsBase}/{Routes.RoundId}/order", UpdateRoundOrderAsync);
        return endpoints;
    }

    [RequireGame, RequireRound]
    [RequireUserOwnsGame]
    [RequireGameState(GameState.Draft)]
    private static async Task UpdateRoundOrderAsync(
        [FromServices] IDbContextFactory<QuibbleDbContext> dbContextFactory,
        [FromBody, Validate] UpdateRoundOrderRequest request,
        RoundContext roundContext,
        GameEvents gameEvents,
        HttpContext httpContext
    )
    {
        var round = roundContext.Round;

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var dbRound = await dbContext.Rounds.FirstAsync(r => r.Id == round.Id);

        dbRound.Order = request.Order;

        await dbContext.SaveChangesAsync();

        await gameEvents
            .SendToEveryone()
            .Round(round)
            .OrderChangedAsync(dbRound.Order);
    }
}
