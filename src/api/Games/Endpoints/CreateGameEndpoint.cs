using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quibble.Api;
using Quibble.Api.Users;
using Quibble.Api.Validation;
using Quibble.Data;
using Quibble.Data.Entites.Games;
using Quibble.Games.Slug;

namespace Quibble.Games.Endpoints;

public class CreateGameRequest
{
    public string Title { get; set; } = string.Empty;

    public class Validator : AbstractValidator<CreateGameRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Title)
                .NotNull()
                .NotEmpty()
                .MaximumLength(GameConstraints.Title.MaxLength);
        }
    }
}

public class CreateGameResponse
{
    public Guid Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
}

public static class CreateGameEndpoint
{
    public static IEndpointRouteBuilder MapCreateGame(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost($"/{Routes.GamesBase}", CreateGameAsync);
        return endpoints;
    }

    private static async Task<CreateGameResponse> CreateGameAsync(
        [FromServices] IGameSlugService gameSlugService,
        [FromServices] IDbContextFactory<QuibbleDbContext> dbContextFactory,
        [FromBody, Validate] CreateGameRequest request,
        UserContext userContext
    )
    {
        var user = userContext.User;

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var game = new Game
        {
            OwnerId = user.Id,
            State = GameState.Draft,
            Title = request.Title,
        };

        await SaveGameAsync(game, gameSlugService, dbContext);

        return new CreateGameResponse
        {
            Id = game.Id,
            Slug = game.Slug,
            Title = game.Title
        };
    }

    private static async Task SaveGameAsync(Game game, IGameSlugService gameSlugService, QuibbleDbContext dbContext)
    {
        dbContext.Games.Add(game);

        for (var i = 0; i < 10; i++)
        {
            game.Slug = gameSlugService.CreateSlug();
            try
            {
                await dbContext.SaveChangesAsync();
                return;
            }
            catch (DbUpdateException)
            {
            }
        }

        throw new InvalidOperationException();
    }
}
