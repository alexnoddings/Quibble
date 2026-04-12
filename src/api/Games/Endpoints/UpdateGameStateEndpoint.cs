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

public class UpdateGameStateRequest
{
    public GameState State { get; set; }
}

public class UpdateGameStateRequestValidator : AbstractValidator<UpdateGameStateRequest>
{
    public UpdateGameStateRequestValidator()
    {
        RuleFor(x => x.State)
            .IsInEnum();
    }
}

public static class UpdateGameStateEndpoint
{
    public static IEndpointRouteBuilder MapUpdateGameState(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut($"/{Routes.GamesBase}/{Routes.GameId}/state", UpdateGameStateAsync);
        return endpoints;
    }

    [RequireGame]
    [RequireUserOwnsGame]
    private static async Task UpdateGameStateAsync(
        [FromServices] IDbContextFactory<QuibbleDbContext> dbContextFactory,
        [FromServices] IGameInfoService gameService,
        [FromBody, Validate] UpdateGameStateRequest request,
        GameContext gameContext,
        GameEvents gameEvents,
        HttpContext httpContext
    )
    {
        var game = gameContext.Game;

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        if (game.State is GameState.Draft)
        {
            var gameHasAtLeastOneQuestion =
                await dbContext.Games
                    .Where(g => g.Id == game.Id)
                    .SelectMany(g => g.Rounds)
                    .SelectMany(r => r.Questions)
                    .AnyAsync();

            if (!gameHasAtLeastOneQuestion)
            {
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            await NormaliseOrderingAsync(dbContext, game.Id);
        }
        else if (game.State is GameState.Open)
        {
            var gameHasAtLeastOneParticipant =
                await dbContext.Games
                    .Where(g => g.Id == game.Id)
                    .SelectMany(g => g.Participants)
                    .AnyAsync();

            if (!gameHasAtLeastOneParticipant)
            {
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }
        }
        else
        {
            // InProgress -> Completed will be executed by the system
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        var dbGame = await dbContext.Games.FirstAsync(g => g.Id == game.Id);
        dbGame.State = request.State;

        await dbContext.SaveChangesAsync();
        await gameEvents.SendToEveryone().Game().StateChangedAsync(dbGame.State);
        await gameService.OnGameStateUpdatedAsync(game.Id);
    }

    private static async Task NormaliseOrderingAsync(QuibbleDbContext dbContext, Guid gameId)
    {
        var rounds =
            await dbContext.Rounds
                .Include(q => q.Questions)
                .Where(r => r.GameId == gameId)
                .ToListAsync();

        var r = 0;
        foreach (var round in rounds.OrderBy(r => r.Order).ToArray())
        {
            round.Order = ++r;
            var q = 0;
            foreach (var question in round.Questions.OrderBy(q => q.Order).ToArray())
            {
                question.Order = ++q;
            }
        }
        await dbContext.SaveChangesAsync();
    }
}
