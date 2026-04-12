using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quibble.Api;
using Quibble.Api.Games;
using Quibble.Api.Users;
using Quibble.Data;
using Quibble.Data.Entites.Games;
using Quibble.Data.Entites.Questions;
using Quibble.Games.Models;

namespace Quibble.Games.Endpoints;

public static class GetGameEndpoint
{
    public static IEndpointRouteBuilder MapGetGame(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"{Routes.GamesBase}/{Routes.GameId}", GetGameAsync);
        return endpoints;
    }

    [RequireGame]
    private static async Task<object?> GetGameAsync(
        [FromServices] IDbContextFactory<QuibbleDbContext> dbContextFactory,
        UserContext userContext,
        GameContext gameContext,
        HttpContext httpContext
    )
    {
        if (Environment.ProcessorCount < -1)
        {
            httpContext.Response.StatusCode = 500;
            return new ProblemDetails
            {
                Type = "https://bad",
                Title = "Thing bad",
                Detail = "The thing was really bad :(",
                Status = 500,
                Instance = "ooeeeeee"
            };
        }

        var user = userContext.User;
        var game = gameContext.Game;

        // Owner can always access the game
        if (game.OwnerId == user.Id)
            return await GetGameAsOwnerAsync(game.Id, dbContextFactory);

        // Only owner can access during draft
        if (game.State is GameState.Draft)
        {
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            return null;
        }

        // User is a participant
        if (game.Participants.Any(p => p.UserId == user.Id))
            return await GetGameAsParticipantAsync(game.Id, user.Id, dbContextFactory);

        // Otherwise, they must join
        httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
        return null;
    }

    private static async Task<FullGame> GetGameAsOwnerAsync(
        Guid gameId,
        IDbContextFactory<QuibbleDbContext> dbContextFactory
    )
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var gameModel =
            await dbContext.Games
                .Where(g => g.Id == gameId)
                .AsSplitQuery()
                .Select(g => new FullGame
                {
                    Id = g.Id,
                    Slug = g.Slug,
                    State = g.State,
                    Title = g.Title,
                    Participants = g.Participants
                        .Select(p => new FullParticipant { Id = p.Id, UserName = p.User.DisplayName })
                        .ToList(),
                    Rounds = g.Rounds
                        .Select(r => new FullRound
                        {
                            Id = r.Id,
                            Order = r.Order,
                            State = r.State,
                            Title = r.Title,
                            Description = r.Description,
                            Questions = r.Questions
                                .Select(q => new FullQuestion
                                {
                                    Id = q.Id,
                                    Order = q.Order,
                                    State = q.State,
                                    Points = q.Points,
                                    Body = new FullQuestionBody { Text = q.Body.Text },
                                    Answer = new FullQuestionAnswer
                                    {
                                        Answer = q.Answer.Answer,
                                        SubmittedAnswers = q.Answer.SubmittedAnswers
                                            .Select(sa => new FullSubmittedAnswer
                                            {
                                                ParticipantId = sa.ParticipantId,
                                                QuestionId = q.Id,
                                                Points = sa.Points,
                                                Answer = sa.Answer,
                                            })
                                            .ToList()
                                    }
                                })
                                .ToList()
                        })
                        .ToList(),
                })
                .FirstAsync();

        return gameModel;
    }

    private static async Task<PartialGame> GetGameAsParticipantAsync(
        Guid gameId,
        Guid userId,
        IDbContextFactory<QuibbleDbContext> dbContextFactory
    )
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var gameModel =
            await dbContext.Games
                .Where(g => g.Id == gameId)
                .AsSplitQuery()
                .Select(g => new PartialGame
                {
                    Id = g.Id,
                    Slug = g.Slug,
                    OwnerName = g.Owner.DisplayName,
                    State = g.State,
                    Title = g.Title,
                    Participants = g.Participants
                        .Select(p => new PartialParticipant
                        {
                            Id = p.Id,
                            UserName = p.User.DisplayName,
                            Points = p.SubmittedAnswers
                                .Where(sa => sa.QuestionAnswer.Question.State == QuestionState.Revealed)
                                .Select(sa => sa.Points ?? 0)
                                .Sum()
                        })
                        .ToList(),
                    Rounds = g.Rounds
                        .Select(r => new PartialRound
                        {
                            Id = r.Id,
                            Order = r.Order,
                            State = r.State,
                            Title = r.Title,
                            Description = r.Description,
                            Questions = r.Questions
                                .Select(q => new PartialQuestion
                                {
                                    Id = q.Id,
                                    Order = q.Order,
                                    State = q.State,
                                    Points = q.Points,
                                    Body = new PartialQuestionBody
                                    {
                                        Text = q.State == QuestionState.Hidden ? null : q.Body.Text
                                    },
                                    Answer = new PartialQuestionAnswer
                                    {
                                        Answer = q.State == QuestionState.Revealed ? q.Answer.Answer : null,
                                        SubmittedAnswer = q.Answer.SubmittedAnswers
                                            .Where(sa => sa.Participant.UserId == userId)
                                            .Select(sa => new PartialSubmittedAnswer
                                            {
                                                Points = q.State == QuestionState.Revealed
                                                    ? sa.Points
                                                    : null,
                                                Answer = sa.Answer,
                                            })
                                            .First()
                                    }
                                })
                                .ToList()
                        })
                        .ToList(),
                })
                .FirstAsync();

        return gameModel;
    }
}
