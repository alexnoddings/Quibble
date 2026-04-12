using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quibble.Api;
using Quibble.Api.Games;
using Quibble.Api.Rounds;
using Quibble.Api.Users;
using Quibble.Api.Validation;
using Quibble.Data;
using Quibble.Data.Entites.Games;
using Quibble.Data.Entites.Questions;
using Quibble.Data.Entites.Questions.Answer;
using Quibble.Data.Entites.Questions.Body;

namespace Quibble.Questions.Endpoints;

public class CreateQuestionRequest
{
    public decimal Points { get; set; }
    public string Body { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
}

public class CreateQuestionRequestValidator : AbstractValidator<CreateQuestionRequest>
{
    public CreateQuestionRequestValidator()
    {
        RuleFor(x => x.Points)
            .GreaterThanOrEqualTo(QuestionConstraints.Points.Minimum)
            .LessThanOrEqualTo(QuestionConstraints.Points.Maximum);

        RuleFor(x => x.Body)
            .NotNull()
            .NotEmpty()
            .MaximumLength(QuestionBodyTextConstraints.Text.MaxLength);

        RuleFor(x => x.Answer)
            .NotNull()
            .NotEmpty()
            .MaximumLength(QuestionAnswerTextConstraints.Answer.MaxLength);
    }
}

public class CreateQuestionResponse
{
    public Guid Id { get; set; }
    public int Order { get; set; }
    public string Body { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public decimal Points { get; set; }
}

public static class CreateQuestionEndpoint
{
    public static IEndpointRouteBuilder MapCreateQuestion(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost($"/{Routes.RoundsBase}/{Routes.RoundId}/questions", CreateQuestionAsync);
        return endpoints;
    }

    [RequireGame, RequireRound]
    [RequireUserOwnsGame]
    [RequireGameState(GameState.Draft)]
    private static async Task<CreateQuestionResponse?> CreateQuestionAsync(
        [FromServices] IDbContextFactory<QuibbleDbContext> dbContextFactory,
        [FromBody, Validate] CreateQuestionRequest request,
        RoundContext roundContext,
        GameEvents gameEvents,
        HttpContext httpContext
    )
    {
        var round = roundContext.Round;

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var order =
            await dbContext.Questions
                .Where(r => r.RoundId == round.Id)
                .CountAsync() + 1;

        var body = new QuestionBody { Text = request.Body };
        var answer = new QuestionAnswer { Answer = request.Answer };
        var points = QuestionConstraints.Points.Clamp(request.Points);

        var question = new Question
        {
            RoundId = round.Id,
            State = QuestionState.Hidden,
            Order = order,
            Points = points,
            Body = body,
            Answer = answer
        };
        dbContext.Questions.Add(question);
        await dbContext.SaveChangesAsync();

        await gameEvents
            .SendToEveryone()
            .Question(question.Id)
            .AddedAsync(
                question.RoundId,
                question.Order,
                question.State,
                question.Points,
                question.Body.Text,
                question.Answer.Answer
            );

        return new CreateQuestionResponse
        {
            Id = question.Id,
            Order = question.Order,
            Points = question.Points,
            Body = body.Text,
            Answer = answer.Answer
        };
    }
}
