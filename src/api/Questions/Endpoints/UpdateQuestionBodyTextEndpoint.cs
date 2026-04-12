using FluentValidation;
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
using Quibble.Data.Entites.Questions.Body;

namespace Quibble.Questions.Endpoints;

public class UpdateQuestionBodyTextDataRequest
{
    public string Text { get; set; } = string.Empty;
}

public class UpdateQuestionBodyTextDataRequestValidator : AbstractValidator<UpdateQuestionBodyTextDataRequest>
{
    public UpdateQuestionBodyTextDataRequestValidator()
    {
        RuleFor(x => x.Text)
            .NotNull()
            .NotEmpty()
            .MaximumLength(QuestionBodyTextConstraints.Text.MaxLength);
    }
}

public static class UpdateQuestionBodyTextEndpoint
{
    public static IEndpointRouteBuilder MapUpdateQuestionBodyText(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut($"/{Routes.QuestionsBase}/{Routes.QuestionId}/body/text", UpdateQuestionBodyTextAsync);
        return endpoints;
    }

    [RequireGame, RequireRound, RequireQuestion]
    [RequireUserOwnsGame]
    [RequireGameState(GameState.Draft)]
    private static async Task UpdateQuestionBodyTextAsync(
        [FromServices] IDbContextFactory<QuibbleDbContext> dbContextFactory,
        [FromBody, Validate] UpdateQuestionBodyTextDataRequest request,
        QuestionContext questionContext,
        GameEvents gameEvents,
        HttpContext httpContext
    )
    {
        var question = questionContext.Question;

        // if (question.BodyType is not QuestionBodyType.Text)
        // {
        //     httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        //     return;
        // }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var dbQuestion = await dbContext.Questions
            .Include(q => q.Body)
            .FirstAsync(q => q.Id == question.Id);

        // if (dbQuestion.Body is not QuestionBodyText bodyText)
        // {
        //     Debug.Fail("Question.Body should be text based on BodyType.");
        //     httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        //     return;
        // }

        dbQuestion.Body.Text = request.Text;

        await dbContext.SaveChangesAsync();

        await gameEvents
            .SendToEveryone()
            .Question(question)
            .BodyTextChangedAsync(dbQuestion.Body.Text);
    }
}
