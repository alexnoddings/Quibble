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

public class UpdateRoundDescriptionRequest
{
    public string Description { get; set; } = string.Empty;
}

public class UpdateRoundDescriptionRequestValidator : AbstractValidator<UpdateRoundDescriptionRequest>
{
    public UpdateRoundDescriptionRequestValidator()
    {
        RuleFor(x => x.Description)
            .NotNull()
            .MaximumLength(RoundConstraints.Description.MaxLength);
    }
}

public static class UpdateRoundDescriptionEndpoint
{
    public static IEndpointRouteBuilder MapUpdateRoundDescription(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut($"/{Routes.RoundsBase}/{Routes.RoundId}/description", UpdateRoundDescriptionAsync);
        return endpoints;
    }

    [RequireGame, RequireRound]
    [RequireUserOwnsGame]
    [RequireGameState(GameState.Draft)]
    private static async Task UpdateRoundDescriptionAsync(
        [FromServices] IDbContextFactory<QuibbleDbContext> dbContextFactory,
        [FromBody, Validate] UpdateRoundDescriptionRequest request,
        RoundContext roundContext,
        GameEvents gameEvents,
        HttpContext httpContext
    )
    {
        var round = roundContext.Round;

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var dbRound = await dbContext.Rounds.FirstAsync(r => r.Id == round.Id);

        dbRound.Description = request.Description;

        await dbContext.SaveChangesAsync();

        await gameEvents
            .SendToEveryone()
            .Round(round)
            .DescriptionChangedAsync(dbRound.Description);
    }
}
