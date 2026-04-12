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
using Quibble.Data.Entites.Questions.Answer;

namespace Quibble.Questions.Endpoints;

public class UpdateQuestionAnswerTextDataRequest
{
    public string Answer { get; set; } = string.Empty;
}

public class UpdateQuestionAnswerTextDataRequestValidator : AbstractValidator<UpdateQuestionAnswerTextDataRequest>
{
    public UpdateQuestionAnswerTextDataRequestValidator()
    {
        RuleFor(x => x.Answer)
            .NotNull()
            .MaximumLength(QuestionAnswerTextConstraints.Answer.MaxLength);
    }
}

public static class UpdateQuestionAnswerTextEndpoint
{
    public static IEndpointRouteBuilder MapUpdateQuestionAnswerText(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut($"/{Routes.QuestionsBase}/{Routes.QuestionId}/answer/text", UpdateQuestionAnswerTextAsync);
        return endpoints;
    }

    [RequireGame, RequireRound, RequireQuestion]
    [RequireUserOwnsGame]
    [RequireGameState(GameState.Draft)]
    private static async Task UpdateQuestionAnswerTextAsync(
        [FromServices] IDbContextFactory<QuibbleDbContext> dbContextFactory,
        [FromBody, Validate] UpdateQuestionAnswerTextDataRequest request,
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

        dbQuestion.Answer.Answer = request.Answer;

        await dbContext.SaveChangesAsync();

        await gameEvents
            .SendToEveryone()
            .Question(question)
            .AnswerTextChangedAsync(dbQuestion.Answer.Answer);
    }
}
