using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quibble.Api;
using Quibble.Api.Games;
using Quibble.Api.Answers;
using Quibble.Api.Users;
using Quibble.Api.Validation;
using Quibble.Data;
using Quibble.Data.Entites.Answers;
using Quibble.Data.Entites.Games;
using Quibble.Data.Entites.Questions;

namespace Quibble.Answers.Endpoints;

public class UpdateAnswerPointsRequest
{
    public decimal Points { get; set; }
}

public class UpdateAnswerPointsRequestValidator : AbstractValidator<UpdateAnswerPointsRequest>
{
    public UpdateAnswerPointsRequestValidator()
    {
        RuleFor(x => x.Points)
            .GreaterThanOrEqualTo(SubmittedAnswerConstraints.Points.Minimum)
            .LessThanOrEqualTo(SubmittedAnswerConstraints.Points.Maximum);
    }
}

public static class UpdateAnswerPointsEndpoint
{
    public static IEndpointRouteBuilder MapUpdateAnswerPoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut(
            $"/{Routes.QuestionsBase}/{Routes.QuestionId}/answers/{Routes.ParticipantId}/points",
            UpdateAnswerPointsAsync
        );
        return endpoints;
    }

    [RequireAnswer]
    [RequireUserOwnsGame]
    [RequireGameState(GameState.InProgress)]
    private static async Task<NoContent> UpdateAnswerPointsAsync(
        [FromServices] IDbContextFactory<QuibbleDbContext> dbContextFactory,
        [FromBody, Validate] UpdateAnswerPointsRequest request,
        AnswerContext answerContext,
        GameEvents gameEvents,
        HttpContext httpContext
    )
    {
        var answer = answerContext.Answer;

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var dbAnswer = await dbContext.Answers
            .Where(a => a.ParticipantId == answer.Participant.Id)
            .Where(a => a.QuestionAnswer.QuestionId == answer.Question.Id)
            .FirstAsync();

        var points = QuestionConstraints.Points.Clamp(request.Points);
        dbAnswer.Points = points;

        await dbContext.SaveChangesAsync();

        await gameEvents
            .SendToHost()
            .Answer(answer)
            .PointsChangedAsync(points);

        await gameEvents
            .SendToParticipant(answerContext.Answer.Participant.Id)
            .Answer(answer)
            .PointsChangedAsync(points);

        return TypedResults.NoContent();
    }
}
