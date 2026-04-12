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
using Quibble.Questions.Endpoints;

namespace Quibble.Answers.Endpoints;

public class UpdateAnswerTextDataRequest
{
    public string Answer { get; set; } = string.Empty;
}

public class UpdateAnswerTextDataRequestValidator : AbstractValidator<UpdateAnswerTextDataRequest>
{
    public UpdateAnswerTextDataRequestValidator()
    {
        RuleFor(x => x.Answer)
            .NotNull()
            .MaximumLength(QuestionAnswerTextConstraints.Answer.MaxLength);
    }
}

public static class UpdateAnswerTextEndpoint
{
    public static IEndpointRouteBuilder MapUpdateAnswerText(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut(
            $"/{Routes.QuestionsBase}/{Routes.QuestionId}/answers/@me/text",
            UpdateAnswerTextAsync
        );
        return endpoints;
    }

    [RequireAnswer]
    [RequireUserIsParticipant]
    [RequireParticipantOwnsAnswer]
    [RequireGameState(GameState.InProgress)]
    private static async Task UpdateAnswerTextAsync(
        [FromServices] IDbContextFactory<QuibbleDbContext> dbContextFactory,
        [FromBody, Validate] UpdateAnswerTextDataRequest request,
        AnswerContext answerContext,
        GameEvents gameEvents,
        HttpContext httpContext
    )
    {
        var answer = answerContext.Answer;

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var dbAnswer = await dbContext.Answers
            .Include(a => a.QuestionAnswer.Question)
            .Where(a => a.QuestionAnswer.QuestionId == answer.Question.Id)
            .Where(a => a.ParticipantId == answer.Participant.Id)
            .FirstAsync();

        if (dbAnswer.QuestionAnswer.Question.State is not QuestionState.InProgress)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        dbAnswer.Answer = request.Answer;

        await dbContext.SaveChangesAsync();

        await gameEvents
            .SendToHost()
            .Answer(answer)
            .TextChangedAsync(dbAnswer.Answer);

        await gameEvents
            .SendToParticipant(answerContext.Answer.Participant.Id)
            .Answer(answer)
            .TextChangedAsync(dbAnswer.Answer);
    }
}
