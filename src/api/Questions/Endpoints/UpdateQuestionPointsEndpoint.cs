using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quibble.Api;
using Quibble.Api.Games;
using Quibble.Api.Questions;
using Quibble.Api.Rounds;
using Quibble.Api.Users;
using Quibble.Api.Validation;
using Quibble.Data;
using Quibble.Data.Entites.Games;
using Quibble.Data.Entites.Questions;

namespace Quibble.Questions.Endpoints;

public class UpdateQuestionPointsRequest
{
    public decimal Points { get; set; }
}

public class UpdateQuestionPointsRequestValidator : AbstractValidator<UpdateQuestionPointsRequest>
{
    public UpdateQuestionPointsRequestValidator()
    {
        RuleFor(x => x.Points)
            .GreaterThanOrEqualTo(QuestionConstraints.Points.Minimum)
            .LessThanOrEqualTo(QuestionConstraints.Points.Maximum);
    }
}

public static class UpdateQuestionPointsEndpoint
{
    public static IEndpointRouteBuilder MapUpdateQuestionPoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut($"/{Routes.QuestionsBase}/{Routes.QuestionId}/points", UpdateQuestionPointsAsync);
        return endpoints;
    }

    [RequireGame, RequireRound, RequireQuestion]
    [RequireUserOwnsGame]
    [RequireGameState(GameState.Draft)]
    private static async Task<NoContent> UpdateQuestionPointsAsync(
        [FromServices] IDbContextFactory<QuibbleDbContext> dbContextFactory,
        [FromBody, Validate] UpdateQuestionPointsRequest request,
        QuestionContext questionContext,
        GameEvents gameEvents,
        HttpContext httpContext
    )
    {
        var question = questionContext.Question;

        // if (question.AnswerType is not QuestionAnswerType.Text)
        // {
        //     httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        //     return;
        // }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var dbQuestion = await dbContext.Questions
            .Include(q => q.Answer)
            .FirstAsync(q => q.Id == question.Id);

        // if (dbQuestion.Answer is not QuestionAnswerText answerText)
        // {
        //     Debug.Fail("Question.Answer should be text based on AnswerType.");
        //     httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        //     return;
        // }

        var points = QuestionConstraints.Points.Clamp(request.Points);
        dbQuestion.Points = points;

        await dbContext.SaveChangesAsync();

        await gameEvents
            .SendToEveryone()
            .Question(question)
            .PointsChangedAsync(dbQuestion.Points);

        return TypedResults.NoContent();
    }
}
