using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quibble.Answers.Similarity;
using Quibble.Api;
using Quibble.Api.Games;
using Quibble.Api.Questions;
using Quibble.Api.Rounds;
using Quibble.Api.Users;
using Quibble.Api.Validation;
using Quibble.Data;
using Quibble.Data.Entites.Answers;
using Quibble.Data.Entites.Games;
using Quibble.Data.Entites.Questions;
using Quibble.Data.Entites.Rounds;
using Quibble.Questions.Info;

namespace Quibble.Questions.Endpoints;

public class UpdateQuestionStateRequest
{
    public QuestionState State { get; set; }
}

public class UpdateQuestionStateRequestValidator : AbstractValidator<UpdateQuestionStateRequest>
{
    public UpdateQuestionStateRequestValidator()
    {
        RuleFor(x => x.State)
            .IsInEnum();
    }
}

public static class UpdateQuestionEndpoint
{
    public static IEndpointRouteBuilder MapUpdateQuestionState(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut($"/{Routes.QuestionsBase}/{Routes.QuestionId}/state", UpdateQuestionStateAsync);
        return endpoints;
    }

    [RequireGame, RequireRound, RequireQuestion]
    [RequireUserOwnsGame]
    [RequireGameState(GameState.InProgress)]
    [RequireRoundState(RoundState.Visible)]
    private static async Task UpdateQuestionStateAsync(
        [FromServices] IDbContextFactory<QuibbleDbContext> dbContextFactory,
        [FromServices] IQuestionInfoService questionService,
        [FromBody, Validate] UpdateQuestionStateRequest request,
        QuestionContext questionContext,
        GameEvents gameEvents,
        HttpContext httpContext
    )
    {
        var question = questionContext.Question;

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var dbQuestion = await dbContext.Questions
            .Include(q => q.Body)
            .Include(q => q.Answer)
            .FirstAsync(r => r.Id == question.Id);

        var isValidTransition = (dbQuestion.State, request.State) switch
        {
            (QuestionState.Hidden, QuestionState.InProgress) => true,
            (QuestionState.InProgress, QuestionState.Marking) => true,
            (QuestionState.Marking, QuestionState.Revealed) => true,
            _ => false
        };

        if (!isValidTransition)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        dbQuestion.State = request.State;

        await dbContext.SaveChangesAsync();
        await questionService.OnQuestionStateUpdatedAsync(question.Id);

        if (request.State is QuestionState.InProgress)
        {
            await gameEvents
                .SendToHost()
                .Question(question)
                .StateChangedAsync(dbQuestion.State);

            await gameEvents
                .SendToParticipants()
                .Question(question)
                .RevealedAsync(dbQuestion.Body.Text, dbQuestion.Answer.Answer);
        }
        else
        {
            await gameEvents
                .SendToEveryone()
                .Question(question)
                .StateChangedAsync(dbQuestion.State);
        }

        if (request.State is QuestionState.Marking)
        {
            var dbAnswers = await dbContext.Answers
                .Where(a => a.QuestionAnswerId == dbQuestion.Id)
                .ToListAsync();
            TryAutoMarkAnswers(dbQuestion, dbAnswers);

            await Task.WhenAll(
                dbAnswers
                    .Where(a => a.Points is not null)
                    .Select(async a =>
                    {
                        await gameEvents
                            .SendToHost()
                            .Answer(a.ParticipantId, dbQuestion.Id)
                            .PointsChangedAsync(a.Points!.Value);

                        await gameEvents
                            .SendToParticipant(a.ParticipantId)
                            .Answer(a.ParticipantId, dbQuestion.Id)
                            .PointsChangedAsync(a.Points!.Value);
                    })
            );
        }
    }

    private static readonly SimilarityCalculator _similarityCalculator = new HammingSimilarityCalculator(0.85);
    private static void TryAutoMarkAnswers(Question question, List<SubmittedAnswer> submittedAnswers)
    {
        var correct = NormalisedAnswer.Normalise(question.Answer.Answer);
        if (correct.Text.Length is 0)
            return;

        foreach (var submittedAnswer in submittedAnswers)
        {
            var answer = NormalisedAnswer.Normalise(submittedAnswer.Answer);
            var isAnswerSimilarToCorrect = _similarityCalculator.AreSimilar(correct, answer);
            if (isAnswerSimilarToCorrect)
                submittedAnswer.Points = question.Points;
            // Leave marking wrong to the person running the game
        }
    }
}
