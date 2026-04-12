using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quibble.Api;
using Quibble.Api.Games;
using Quibble.Api.Users;
using Quibble.Api.Validation;
using Quibble.Data;
using Quibble.Data.Entites.Games;
using Quibble.Data.Entites.Rounds;

namespace Quibble.Rounds.Endpoints;

public class CreateRoundRequest
{
    public string Title { get; set; } = string.Empty;
}

public class CreateRoundRequestValidator : AbstractValidator<CreateRoundRequest>
{
    public CreateRoundRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotNull()
            .NotEmpty()
            .MaximumLength(RoundConstraints.Title.MaxLength);
    }
}

public class CreateRoundResponse
{
    public Guid Id { get; set; }
    public RoundState State { get; set; }
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public static class CreateRoundEndpoint
{
    public static IEndpointRouteBuilder MapCreateRound(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost($"/{Routes.GamesBase}/{Routes.GameId}/rounds", CreateRoundAsync);
        return endpoints;
    }

    [RequireGame]
    [RequireUserOwnsGame]
    [RequireGameState(GameState.Draft)]
    private static async Task<CreateRoundResponse?> CreateRoundAsync(
        [FromServices] IDbContextFactory<QuibbleDbContext> dbContextFactory,
        [FromBody, Validate] CreateRoundRequest request,
        GameContext gameContext,
        GameEvents gameEvents,
        HttpContext httpContext
    )
    {
        var game = gameContext.Game;

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var order =
            await dbContext.Rounds
                .Where(r => r.GameId == game.Id)
                .CountAsync() + 1;

        var round = new Round
        {
            GameId = game.Id,
            State = RoundState.Hidden,
            Order = order,
            Title = request.Title,
            Description = string.Empty
        };
        dbContext.Rounds.Add(round);
        await dbContext.SaveChangesAsync();

        await gameEvents
            .SendToEveryone()
            .Round(round.Id)
            .AddedAsync(round.Order, round.State, round.Title, round.Description);

        return new CreateRoundResponse
        {
            Id = round.Id,
            Order = round.Order,
            State = round.State,
            Title = round.Title,
            Description = round.Description
        };
    }
}
