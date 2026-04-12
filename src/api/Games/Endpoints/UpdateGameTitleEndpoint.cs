using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quibble.Api;
using Quibble.Api.Games;
using Quibble.Api.Users;
using Quibble.Api.Validation;
using Quibble.Data;
using Quibble.Data.Entites.Games;
using Quibble.Games.Info;

namespace Quibble.Games.Endpoints;

public class UpdateGameTitleRequest
{
    public string Title { get; set; } = string.Empty;
}

public class UpdateGameTitleRequestValidator : AbstractValidator<UpdateGameTitleRequest>
{
    public UpdateGameTitleRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotNull()
            .NotEmpty()
            .MaximumLength(GameConstraints.Title.MaxLength);
    }
}

public static class UpdateGameTitleEndpoint
{
    public static IEndpointRouteBuilder MapUpdateGameTitle(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut($"/{Routes.GamesBase}/{Routes.GameId}/title", UpdateGameTitleAsync);
        return endpoints;
    }

    [RequireGame]
    [RequireUserOwnsGame]
    [RequireGameState(GameState.Draft)]
    private static async Task UpdateGameTitleAsync(
        [FromServices] IDbContextFactory<QuibbleDbContext> dbContextFactory,
        [FromServices] IGameInfoService gameService,
        [FromBody, Validate] UpdateGameTitleRequest request,
        GameContext gameContext,
        GameEvents gameEvents
    )
    {
        var game = gameContext.Game;

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var dbGame = await dbContext.Games.FirstAsync(g => g.Id == game.Id);
        dbGame.Title = request.Title;

        await dbContext.SaveChangesAsync();

        await gameEvents
            .SendToEveryone()
            .Game()
            .TitleChangedAsync(request.Title);
    }
}
