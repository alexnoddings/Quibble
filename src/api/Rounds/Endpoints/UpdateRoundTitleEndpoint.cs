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

public class UpdateRoundTitleRequest
{
    public string Title { get; set; } = string.Empty;
}

public class UpdateRoundTitleRequestValidator : AbstractValidator<UpdateRoundTitleRequest>
{
    public UpdateRoundTitleRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotNull()
            .MaximumLength(RoundConstraints.Title.MaxLength);
    }
}

public static class UpdateRoundTitleEndpoint
{
    public static IEndpointRouteBuilder MapUpdateRoundTitle(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut($"/{Routes.RoundsBase}/{Routes.RoundId}/title", UpdateRoundTitleAsync);
        return endpoints;
    }

    [RequireGame, RequireRound]
    [RequireUserOwnsGame]
    [RequireGameState(GameState.Draft)]
    private static async Task UpdateRoundTitleAsync(
        [FromServices] IDbContextFactory<QuibbleDbContext> dbContextFactory,
        [FromBody, Validate] UpdateRoundTitleRequest request,
        RoundContext roundContext,
        GameEvents gameEvents
    )
    {
        var round = roundContext.Round;

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var dbRound = await dbContext.Rounds.FirstAsync(r => r.Id == round.Id);

        dbRound.Title = request.Title;

        await dbContext.SaveChangesAsync();

        await gameEvents
            .SendToEveryone()
            .Round(round)
            .TitleChangedAsync(dbRound.Title);
    }
}
