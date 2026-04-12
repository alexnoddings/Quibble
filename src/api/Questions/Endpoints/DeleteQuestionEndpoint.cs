using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quibble.Api;
using Quibble.Api.Games;
using Quibble.Api.Questions;
using Quibble.Api.Rounds;
using Quibble.Api.Users;
using Quibble.Data;
using Quibble.Data.Entites.Games;
using Quibble.Data.Entites.Questions;

namespace Quibble.Questions.Endpoints;

public static class DeleteQuestionEndpoint
{
    public static IEndpointRouteBuilder MapDeleteQuestion(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete($"/{Routes.QuestionsBase}/{Routes.QuestionId}", DeleteQuestionAsync);
        return endpoints;
    }

    [RequireGame, RequireRound, RequireQuestion]
    [RequireUserOwnsGame]
    [RequireGameState(GameState.Draft)]
    private static async Task DeleteQuestionAsync(
        [FromServices] IDbContextFactory<QuibbleDbContext> dbContextFactory,
        QuestionContext questionContext,
        GameEvents gameEvents,
        HttpContext httpContext
    )
    {
        var question = questionContext.Question;

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var questionExists = await dbContext.Questions.AnyAsync(r => r.Id == question.Id);
        if (!questionExists)
        {
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        dbContext.Questions.Add(new Question { Id = question.Id }).State = EntityState.Deleted;

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return;
        }

        await gameEvents
            .SendToEveryone()
            .Question(question)
            .RemovedAsync();
    }
}
