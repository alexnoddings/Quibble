using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Quibble.Common.Api;
using Quibble.Common.Dtos;
using Quibble.Common.Entities;
using Quibble.Server.Core.Domain;
using Quibble.Server.Core.Sync;
using Quibble.Sync.SignalR.Shared;
using System.Runtime.CompilerServices;

namespace Quibble.Sync.SignalR.Server;

public class SignalrSyncHub : Hub<ISyncedEvents>
{
	private ILogger<SignalrSyncHub> Logger { get; }
	private QuizLogic Quizzes { get; }
	private RoundLogic Rounds { get; }
	private QuestionLogic Questions { get; }
	private SubmittedAnswerLogic SubmittedAnswers { get; }

	public SignalrSyncHub(ILogger<SignalrSyncHub> logger, QuizLogic quizzes, RoundLogic rounds, QuestionLogic questions, SubmittedAnswerLogic submittedAnswers)
	{
		Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		Quizzes = quizzes ?? throw new ArgumentNullException(nameof(quizzes));
		Rounds = rounds ?? throw new ArgumentNullException(nameof(rounds));
		Questions = questions ?? throw new ArgumentNullException(nameof(questions));
		SubmittedAnswers = submittedAnswers ?? throw new ArgumentNullException(nameof(submittedAnswers));
	}

	public override async Task OnConnectedAsync()
	{
		var contextResult = GetExecutionContext();
		if (!contextResult.WasSuccessful)
		{
			Context.Abort();
			return;
		}

		var allowConnection = await Quizzes.OnUserConnectedAsync(contextResult.Value);
		if (!allowConnection)
		{
			Context.Abort();
			return;
		}

		await base.OnConnectedAsync();
	}

	#region Quiz
	[HubMethodName(SignalrEndpoints.GetQuiz)]
	public Task<ApiResponse<FullQuizDto>> GetQuizAsync() =>
		ExecuteAsync(ctx => Quizzes.GetQuizAsync(ctx));

	[HubMethodName(SignalrEndpoints.OpenQuiz)]
	public Task<ApiResponse> OpenQuizAsync() =>
		ExecuteAsync(ctx => Quizzes.OpenQuizAsync(ctx));

	[HubMethodName(SignalrEndpoints.UpdateQuizTitle)]
	public Task<ApiResponse> UpdateQuizTitleAsync(string newTitle) =>
		ExecuteAsync(ctx => Quizzes.UpdateQuizTitleAsync(ctx, newTitle));

	[HubMethodName(SignalrEndpoints.DeleteQuiz)]
	public Task<ApiResponse> DeleteQuizAsync() =>
		ExecuteAsync(ctx => Quizzes.DeleteQuizAsync(ctx));
	#endregion

	#region Round
	[HubMethodName(SignalrEndpoints.CreateRound)]
	public Task<ApiResponse> CreateRoundAsync() =>
		ExecuteAsync(ctx => Rounds.CreateRoundAsync(ctx));

	[HubMethodName(SignalrEndpoints.UpdateRoundTitle)]
	public Task<ApiResponse> UpdateRoundTitleAsync(Guid roundId, string newTitle) =>
		ExecuteAsync(ctx => Rounds.UpdateRoundTitleAsync(ctx, roundId, newTitle));

	[HubMethodName(SignalrEndpoints.OpenRound)]
	public Task<ApiResponse> OpenRoundAsync(Guid roundId) =>
		ExecuteAsync(ctx => Rounds.OpenRoundAsync(ctx, roundId));

	[HubMethodName(SignalrEndpoints.DeleteRound)]
	public Task<ApiResponse> DeleteRoundAsync(Guid roundId) =>
		ExecuteAsync(ctx => Rounds.DeleteRoundAsync(ctx, roundId));
	#endregion

	#region Question
	[HubMethodName(SignalrEndpoints.CreateQuestion)]
	public Task<ApiResponse> CreateQuestionAsync(Guid roundId) =>
		ExecuteAsync(ctx => Questions.CreateQuestionAsync(ctx, roundId));

	[HubMethodName(SignalrEndpoints.UpdateQuestionText)]
	public Task<ApiResponse> UpdateQuestionTextAsync(Guid questionId, string newText) =>
		ExecuteAsync(ctx => Questions.UpdateQuestionTextAsync(ctx, questionId, newText));

	[HubMethodName(SignalrEndpoints.UpdateQuestionAnswer)]
	public Task<ApiResponse> UpdateQuestionAnswerAsync(Guid questionId, string newAnswer) =>
		ExecuteAsync(ctx => Questions.UpdateQuestionAnswerAsync(ctx, questionId, newAnswer));

	[HubMethodName(SignalrEndpoints.UpdateQuestionPoints)]
	public Task<ApiResponse> UpdateQuestionPointsAsync(Guid questionId, decimal newPoints) =>
		ExecuteAsync(ctx => Questions.UpdateQuestionPointsAsync(ctx, questionId, newPoints));

	[HubMethodName(SignalrEndpoints.UpdateQuestionState)]
	public Task<ApiResponse> UpdateQuestionStateAsync(Guid questionId, QuestionState newState) =>
		ExecuteAsync(ctx => Questions.UpdateQuestionStateAsync(ctx, questionId, newState));

	[HubMethodName(SignalrEndpoints.DeleteQuestion)]
	public Task<ApiResponse> DeleteQuestionAsync(Guid questionId) =>
		ExecuteAsync(ctx => Questions.DeleteQuestionAsync(ctx, questionId));
	#endregion

	#region Answer
	[HubMethodName(SignalrEndpoints.PreviewUpdateSubmittedAnswerText)]
	public Task<ApiResponse> PreviewUpdateSubmittedAnswerTextAsync(Guid answerId, string newText) =>
		ExecuteAsync(ctx => SubmittedAnswers.PreviewUpdateSubmittedAnswerTextAsync(ctx, answerId, newText));

	[HubMethodName(SignalrEndpoints.UpdateSubmittedAnswerText)]
	public Task<ApiResponse> UpdateSubmittedAnswerText(Guid answerId, string newText) =>
		ExecuteAsync(ctx => SubmittedAnswers.UpdateSubmittedAnswerTextAsync(ctx, answerId, newText));

	[HubMethodName(SignalrEndpoints.UpdateSubmittedAnswerAssignedPoints)]
	public Task<ApiResponse> UpdateSubmittedAnswerAssignedPoints(Guid answerId, decimal points) =>
		ExecuteAsync(ctx => SubmittedAnswers.UpdateSubmittedAnswerAssignedPointsAsync(ctx, answerId, points));
	#endregion

	private async Task<ApiResponse> ExecuteAsync(Func<SignalrExecutionContext, Task<ApiResponse>> func, [CallerMemberName] string caller = "")
	{
		var contextResult = GetExecutionContext();
		if (contextResult.WasSuccessful)
		{
			Logger.LogDebug("Executing {EndpointMethod}.", caller);
			return await func(contextResult.Value);
		}

		Logger.LogDebug("Cannot execute {EndpointMethod}: {Error}.", caller, contextResult.Error);
		return ApiResponse.FromError(contextResult.Error);
	}

	private async Task<ApiResponse<TResult>> ExecuteAsync<TResult>(Func<SignalrExecutionContext, Task<ApiResponse<TResult>>> func, [CallerMemberName] string caller = "")
	{
		var contextResult = GetExecutionContext();
		if (contextResult.WasSuccessful)
		{
			Logger.LogDebug("Executing {EndpointMethod}.", caller);
			return await func(contextResult.Value);
		}

		Logger.LogDebug("Cannot execute {EndpointMethod}: {Error}.", caller, contextResult.Error);
		return ApiResponse.FromError<TResult>(contextResult.Error);
	}

	private ApiResponse<SignalrExecutionContext> GetExecutionContext()
	{
		if (GetUserId() is not { } userId)
			return ApiResponse.FromError<SignalrExecutionContext>(new ApiError(401, "UserNotFound"));

		if (GetQuizId() is not { } quizId)
			return ApiResponse.FromError<SignalrExecutionContext>(new ApiError(404, "QuizNotFound"));

		var users = new SignalrSyncedUsers(quizId, this);
		var context = new SignalrExecutionContext(userId, quizId, users);
		return ApiResponse.FromSuccess(context);
	}

	private Guid? GetUserId() =>
		Context.UserIdentifier is null
		|| !Guid.TryParse(Context.UserIdentifier, out Guid userId)
		? null
		: userId;

	private Guid? GetQuizId()
	{
		var httpContext = Context.GetHttpContext();
		var quizIdObject = httpContext?.Request.RouteValues["QuizId"];
		return quizIdObject is not string quizIdString
			|| !Guid.TryParse(quizIdString, out Guid quizId)
			? null
			: quizId;
	}
}
