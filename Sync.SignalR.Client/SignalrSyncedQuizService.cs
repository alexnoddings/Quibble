using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Client.Core.Contexts;
using Quibble.Client.Core.Factories;
using Quibble.Common.Api;
using Quibble.Common.Dtos;
using Quibble.Common.Entities;
using Quibble.Sync.SignalR.Client.Contexts;
using Quibble.Sync.SignalR.Shared;
using System.Net;
using System.Net.Http.Json;

namespace Quibble.Sync.SignalR.Client;

internal class SignalrSyncedQuizService : ISyncedQuizService
{
	private ILogger<SignalrSyncedQuizService> Logger { get; }
	private ILoggerFactory LoggerFactory { get; }
	private HttpClient HttpClient { get; }
	private NavigationManager NavigationManager { get; }

	public SignalrSyncedQuizService(ILogger<SignalrSyncedQuizService> logger, ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory, NavigationManager navigationManager)
	{
		Logger = logger;
		LoggerFactory = loggerFactory;
		HttpClient = httpClientFactory.CreateClient("QuizApi");
		NavigationManager = navigationManager;
	}

	public async Task<ApiResponse<(FullQuizDto, ISyncContext)>> GetQuizAsync(Guid id)
	{
		Logger.LogInformation("Getting quiz {QuizId}.", id);

		if (id == Guid.Empty)
		{
			Logger.LogWarning("Quiz ID was empty.");
			return ApiResponse.FromError<(FullQuizDto, ISyncContext)>(ApiErrors.QuizNotFound);
		}

		Logger.LogDebug("Negotiating quiz.");
		var quizNegotiationResponse = await HttpClient.GetAsync($"{id}/negotiate");

		Logger.LogDebug("Negotiation returned {NegotiationStatusCode}.", quizNegotiationResponse.StatusCode);
		if (quizNegotiationResponse.StatusCode == HttpStatusCode.NotFound)
			return ApiResponse.FromError<(FullQuizDto, ISyncContext)>(ApiErrors.QuizNotFound);

		var quizNegotiation = await quizNegotiationResponse.Content.ReadFromJsonAsync<QuizNegotiationDto>();

		if (quizNegotiation is null)
		{
			Logger.LogWarning("Quiz Negotiation was null.");
			return ApiResponse.FromError<(FullQuizDto, ISyncContext)>(ApiErrors.QuizNotFound);
		}

		Logger.LogDebug("Negotiation result: {{ CanEdit: {CanEditQuiz}, State: {QuizState} }}.", quizNegotiation.CanEdit, quizNegotiation.State);

		if (quizNegotiation.State == QuizState.InDevelopment && !quizNegotiation.CanEdit)
		{
			Logger.LogInformation("Quiz is in development but current user is unable to edit it.");
			return ApiResponse.FromError<(FullQuizDto, ISyncContext)>(ApiErrors.QuizNotOpen);
		}

		Logger.LogDebug("Building SignalR connection to hub.");
		var hubUrl = NavigationManager.ToAbsoluteUri($"_sync/{id}");
		var hubConnection =
			new HubConnectionBuilder()
				.WithUrl(hubUrl)
				.WithAutomaticReconnect()
				.Build();

		Logger.LogDebug("Starting SignalR connection.");
		await hubConnection.StartAsync();

		var getQuizHubResponse = await hubConnection.InvokeAsync<ApiResponse<FullQuizDto>>(SignalrEndpoints.GetQuiz);
		if (!getQuizHubResponse.WasSuccessful)
		{
			Logger.LogWarning("Error from quiz hub: [{ErrorCode}] {HubError}.", getQuizHubResponse.Error.StatusCode, getQuizHubResponse.Error.ErrorKey);
			return ApiResponse.FromError<(FullQuizDto, ISyncContext)>(getQuizHubResponse.Error);
		}

		if (getQuizHubResponse.Value is null)
		{
			Logger.LogWarning("Invalid (empty) response from quiz hub.");
			return ApiResponse.FromError<(FullQuizDto, ISyncContext)>(ApiErrors.QuizNotFound);
		}

		var quizDto = getQuizHubResponse.Value;

		Logger.LogDebug("Building SignalR sync context.");
		var context = new SignalrSyncContext(LoggerFactory, hubConnection);

		Logger.LogInformation("Found quiz {QuizId}: {QuizTitle}.", id, quizDto.Quiz.Title);
		return ApiResponse.FromSuccess<(FullQuizDto, ISyncContext)>((quizDto, context));
	}
}
