using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quibble.Api;
using Quibble.Api.Answers;
using Quibble.Api.Games;
using Quibble.Api.Users;
using Quibble.Api.Validation;
using Quibble.Data;
using Quibble.Data.Entites.Games;
using Quibble.Data.Entites.Questions;
using Quibble.Data.Entites.Questions.Answer;

namespace Quibble.Answers.Endpoints;

public class PreviewAnswerTextDataRequest
{
    public string Answer { get; set; } = string.Empty;
}

public class PreviewAnswerTextDataRequestValidator : AbstractValidator<PreviewAnswerTextDataRequest>
{
    public PreviewAnswerTextDataRequestValidator()
    {
        RuleFor(x => x.Answer)
            .NotNull()
            .MaximumLength(QuestionAnswerTextConstraints.Answer.MaxLength);
    }
}

public static class PreviewAnswerTextEndpoint
{
    public static IEndpointRouteBuilder MapPreviewAnswerText(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut(
            $"/{Routes.QuestionsBase}/{Routes.QuestionId}/answers/@me/text/preview",
            PreviewAnswerTextAsync
        );
        return endpoints;
    }

    [RequireAnswer]
    [RequireUserIsParticipant]
    [RequireParticipantOwnsAnswer]
    [RequireGameState(GameState.InProgress)]
    private static async Task PreviewAnswerTextAsync(
        [FromServices] IDbContextFactory<QuibbleDbContext> dbContextFactory,
        [FromBody, Validate] PreviewAnswerTextDataRequest request,
        AnswerContext answerContext,
        GameEvents gameEvents,
        HttpContext httpContext
    )
    {
        var answer = answerContext.Answer;
        if (answer.Question.State is not QuestionState.InProgress)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        await gameEvents
            .SendToHost()
            .Answer(answer)
            .TextPreviewedAsync(request.Answer);

        // Previews aren't sent to the participant
    }
}
