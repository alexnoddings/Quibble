using Quibble.Api.Answers;
using Quibble.Api.Context;
using Quibble.Api.Users;
using Quibble.Games.Info;
using Quibble.Questions.Info;
using Quibble.Rounds.Info;
using Quibble.Users;

namespace Quibble.Api;

public sealed class ContextInjectorMiddleware
{
    private readonly RequestDelegate _next;

    public ContextInjectorMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task InvokeAsync(HttpContext context)
    {
        if (ShouldInjectMetadata(context))
            return InjectThenNextAsync(context);

        return _next(context);
    }

    private async Task InjectThenNextAsync(HttpContext context)
    {
        var didInjectContext = await ContextInjector.InjectAsync(context);
        if (!didInjectContext)
        {
            // If we couldn't inject context, it was probably because the
            // entity we're trying to inject the context for doesn't exist
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        await _next(context);
    }

    private static bool ShouldInjectMetadata(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint is null)
            return false;

        return endpoint.Metadata.GetMetadata<InjectContextAttribute>() is not null;
    }
}

internal static class ContextInjector
{
    public static IApplicationBuilder UseContextInjector(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ContextInjectorMiddleware>();
    }

    private const string ContextInjectedKey = "QuibbleContextInjected";
    private static readonly object _contextInjectedValue = new();

    public static void EnsureContextInjected(HttpContext httpContext)
    {
        if (!httpContext.Items.ContainsKey(ContextInjectedKey))
            throw new InvalidOperationException("Context has not been injected.");
    }

    // Services are resolved by individual methods rather than being injected
    // to avoid constructing multiple services then only using one.
    public static ValueTask<bool> InjectAsync(HttpContext httpContext)
    {
        if (RouteHasId(Routes.GameIdKey, out var gameId))
            return InjectContextFromGameAsync(httpContext, gameId);

        if (RouteHasId(Routes.RoundIdKey, out var roundId))
            return InjectContextFromRoundAsync(httpContext, roundId);

        if (RouteHasId(Routes.QuestionIdKey, out var questionId))
        {
            var endpointMetadata = httpContext.GetEndpoint()?.Metadata;
            // Check if the endpoint is for an answer
            if (endpointMetadata?.GetMetadata<RequireAnswerAttribute>() is not null)
            {
                // Are we expecting the game owner to call the endpoint?
                var expectOwner = endpointMetadata.GetMetadata<RequireUserOwnsGameAttribute>() is not null;
                if (expectOwner)
                {
                    // Then the endpoint needs to have a participant id
                    if (RouteHasId(Routes.ParticipantIdKey, out var participantId))
                        return InjectContextFromAnswerAsync(httpContext, questionId, participantId);

                    // Endpoints intended to be called by the game owner should
                    // have the answer's participant's id in the route.
                    ContextInjectorLogging.CouldNotIdentifyParticipant(httpContext);
                    return ValueTask.FromResult(false);
                }

                // Are we expecting a participant to call the endpoint?
                var expectParticipant = endpointMetadata.GetMetadata<RequireUserIsParticipantAttribute>() is not null;
                if (expectParticipant)
                {
                    // Then we use the current user as the participant
                    return InjectContextFromAnswerByCurrentUserAsParticipantAsync(httpContext, questionId);
                }

                // Answer endpoints must be called by the game owner or a participant
                ContextInjectorLogging.CouldNotIdentifyParticipant(httpContext);
                return ValueTask.FromResult(false);
            }

            return InjectContextFromQuestionAsync(httpContext, questionId);
        }

        // Context injection requires a game, round, or question id to load the context
        ContextInjectorLogging.CouldNotInjectContext(httpContext);
        return ValueTask.FromResult(false);

        bool RouteHasId(string key, out Guid id) =>
            httpContext.Request.RouteValues.TryGetValue(key, out id);
    }

    private static void MarkContextAsInjectionAttempted(HttpContext httpContext) =>
        httpContext.Items[ContextInjectedKey] = _contextInjectedValue;

    private static async ValueTask<bool> InjectContextFromGameAsync(HttpContext httpContext, Guid gameId)
    {
        var gameService = httpContext.RequestServices.GetRequiredService<IGameInfoService>();
        var gameInfo = await gameService.GetGameByIdAsync(gameId);
        if (gameInfo is null)
            return false;

        var quibbleContext = httpContext.QuibbleContext;
        quibbleContext.Game = gameInfo;

        MarkContextAsInjectionAttempted(httpContext);
        return true;
    }

    private static async ValueTask<bool> InjectContextFromRoundAsync(HttpContext httpContext, Guid roundId)
    {
        var roundService = httpContext.RequestServices.GetRequiredService<IRoundInfoService>();
        var roundInfo = await roundService.GetRoundByIdAsync(roundId);
        if (roundInfo is null)
            return false;

        var quibbleContext = httpContext.QuibbleContext;
        quibbleContext.Round = roundInfo;
        quibbleContext.Game = roundInfo.Game;

        MarkContextAsInjectionAttempted(httpContext);
        return true;
    }

    private static async ValueTask<bool> InjectContextFromQuestionAsync(HttpContext httpContext, Guid questionId)
    {
        var questionService = httpContext.RequestServices.GetRequiredService<IQuestionInfoService>();
        var questionInfo = await questionService.GetQuestionByIdAsync(questionId);
        if (questionInfo is null)
            return false;

        var quibbleContext = httpContext.QuibbleContext;
        quibbleContext.Question = questionInfo;
        quibbleContext.Round = questionInfo.Round;
        quibbleContext.Game = questionInfo.Round.Game;

        MarkContextAsInjectionAttempted(httpContext);
        return true;
    }

    private static async ValueTask<bool> InjectContextFromAnswerAsync(
        HttpContext httpContext,
        Guid questionId,
        Guid participantId
    )
    {
        var answerService = httpContext.RequestServices.GetRequiredService<IAnswerInfoService>();
        var answerInfo = await answerService.GetAnswerToQuestionByParticipantAsync(questionId, participantId);
        if (answerInfo is null)
            return false;

        var quibbleContext = httpContext.QuibbleContext;
        quibbleContext.Answer = answerInfo;
        quibbleContext.Question = answerInfo.Question;
        quibbleContext.Round = answerInfo.Question.Round;
        quibbleContext.Game = answerInfo.Question.Round.Game;

        MarkContextAsInjectionAttempted(httpContext);
        return true;
    }

    private static async ValueTask<bool> InjectContextFromAnswerByCurrentUserAsParticipantAsync(
        HttpContext httpContext,
        Guid questionId
    )
    {
        var userService = httpContext.RequestServices.GetRequiredService<ICurrentUserService>();
        var userInfo = await userService.GetCurrentUserInfoAsync();

        var answerService = httpContext.RequestServices.GetRequiredService<IAnswerInfoService>();
        var answerInfo = await answerService.GetAnswerToQuestionByUserAsync(questionId, userInfo.Id);
        if (answerInfo is null)
            return false;

        var quibbleContext = httpContext.QuibbleContext;
        quibbleContext.Answer = answerInfo;
        quibbleContext.Question = answerInfo.Question;
        quibbleContext.Round = answerInfo.Question.Round;
        quibbleContext.Game = answerInfo.Question.Round.Game;

        MarkContextAsInjectionAttempted(httpContext);
        return true;
    }
}

internal ref struct HttpContextQuibbleContext
{
    private readonly IDictionary<object, object?> _httpItems;

    public HttpContextQuibbleContext(IDictionary<object, object?> httpItems)
    {
        _httpItems = httpItems;
    }

    public GameInfo? Game
    {
        get => _httpItems[typeof(GameInfo)] as GameInfo;
        set => _httpItems[typeof(GameInfo)] = value;
    }
    public RoundInfo? Round
    {
        get => _httpItems[typeof(RoundInfo)] as RoundInfo;
        set => _httpItems[typeof(RoundInfo)] = value;
    }
    public QuestionInfo? Question
    {
        get => _httpItems[typeof(QuestionInfo)] as QuestionInfo;
        set => _httpItems[typeof(QuestionInfo)] = value;
    }
    public AnswerInfo? Answer
    {
        get => _httpItems[typeof(AnswerInfo)] as AnswerInfo;
        set => _httpItems[typeof(AnswerInfo)] = value;
    }
    public GameParticipantInfo? Participant
    {
        get => _httpItems[typeof(GameParticipantInfo)] as GameParticipantInfo;
        set => _httpItems[typeof(GameParticipantInfo)] = value;
    }
}

internal static class HttpContextQuibbleContextExtensions
{
    extension(HttpContext httpContext)
    {
        public HttpContextQuibbleContext QuibbleContext => new(httpContext.Items);
    }
}

internal static partial class ContextInjectorLogging
{
    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Could not identify participant for endpoint {Endpoint}."
    )]
    private static partial void CouldNotIdentifyParticipant(ILogger logger, Endpoint? endpoint);

    internal static void CouldNotIdentifyParticipant(HttpContext httpContext)
    {
        var logger = TryGetLogger(httpContext);
        if (logger is null)
            return;

        var endpoint = httpContext.GetEndpoint();
        CouldNotIdentifyParticipant(logger, endpoint);
    }

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Could not inject context to endpoint '{Endpoint}'."
    )]
    private static partial void CouldNotInjectContext(ILogger logger, Endpoint? endpoint);

    internal static void CouldNotInjectContext(HttpContext httpContext)
    {
        var logger = TryGetLogger(httpContext);
        if (logger is null)
            return;

        var endpoint = httpContext.GetEndpoint();
        CouldNotInjectContext(logger, endpoint);
    }

    private static ILogger? TryGetLogger(HttpContext httpContext)
    {
        var loggerFactory = httpContext.RequestServices.GetService<ILoggerFactory>();
        return loggerFactory?.CreateLogger(typeof(ContextInjector));
    }
}
