using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Quibble.Api;
using Quibble.Api.Games;
using Quibble.Api.Users;
using Quibble.Data;
using Quibble.Data.Entites.Answers;
using Quibble.Data.Entites.Games;
using Quibble.Games.Info;

namespace Quibble.Games.Endpoints;

public static class JoinGameEndpoint
{
    public static IEndpointRouteBuilder MapJoinGame(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost($"/{Routes.GamesBase}/{Routes.GameId}/join", JoinGameAsync);
        return endpoints;
    }

    [RequireGame]
    private static async Task JoinGameAsync(
        [FromServices] IDbContextFactory<QuibbleDbContext> dbContextFactory,
        [FromServices] IGameInfoService gameService,
        GameContext gameContext,
        UserContext userContext,
        GameEvents gameEvents,
        HttpContext httpContext
    )
    {
        var game = gameContext.Game;
        var user = userContext.User;

        var isOwner = game.OwnerId == user.Id;
        if (isOwner)
        {
            // Can't join own quiz
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        var isParticipant = game.Participants.Any(p => p.UserId == user.Id);
        if (isParticipant)
        {
            // Already joined
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        if (game.State != GameState.Open)
        {
            // State error
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

       var gameQuestionsIds = await dbContext
            .Questions
            .Where(q => q.Round.GameId == game.Id)
            .Select(q => q.Id)
            .ToListAsync();

        var submittedAnswers = gameQuestionsIds
            .Select(q => new SubmittedAnswer
            {
                Answer = string.Empty,
                Points = null,
                QuestionAnswerId = q
            })
            .ToList();

        var participant = new Participant
        {
            GameId = game.Id,
            UserId = user.Id,
            SubmittedAnswers = submittedAnswers
        };

        dbContext.Participants.Add(participant);

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            // User sent multiple requests to join, and another completed
            // between our isParticipant check and here.
            // They've already been added, so there's nothing to do now.
            httpContext.Response.StatusCode = StatusCodes.Status204NoContent;
            return;
        }

        // Send user and their empty answers to the host
        await gameEvents
            .SendToHost()
            .Participant(participant.Id)
            .AddedAsync(user.DisplayName, participant.SubmittedAnswers);

        // And just the user to participants
        await gameEvents
            .SendToParticipants()
            .Participant(participant.Id)
            .AddedAsync(user.DisplayName);

        await gameService.OnGameParticipantsChangedAsync(game.Id);

        httpContext.Response.StatusCode = StatusCodes.Status201Created;
    }
}
