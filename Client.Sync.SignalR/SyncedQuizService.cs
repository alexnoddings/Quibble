using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Contexts;
using Quibble.Client.Sync.Core;
using Quibble.Client.Sync.SignalR.Contexts;
using Quibble.Shared.Api;
using Quibble.Shared.Entities;
using Quibble.Shared.Models.Dtos;
using Quibble.Shared.Sync.SignalR;
using System.Net;
using System.Net.Http.Json;

namespace Quibble.Client.Sync.SignalR
{
    internal class SyncedQuizService : ISyncedQuizService
    {
        private ILogger<SyncedQuizService> Logger { get; }
        private ILoggerFactory LoggerFactory { get; }
        private HttpClient HttpClient { get; }
        private NavigationManager NavigationManager { get; }

        public SyncedQuizService(ILogger<SyncedQuizService> logger, ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory, NavigationManager navigationManager)
        {
            Logger = logger;
            LoggerFactory = loggerFactory;
            HttpClient = httpClientFactory.CreateClient("QuizApi");
            NavigationManager = navigationManager;
        }

        public async Task<ApiResponse<(FullQuizDto, ISyncContext)>> GetQuizAsync(Guid id)
        {
            if (id == Guid.Empty)
                return ApiResponse.FromError<(FullQuizDto, ISyncContext)>(ApiErrors.QuizNotFound);

            var quizNegotiationResponse = await HttpClient.GetAsync($"{id}/negotiate");
            if (quizNegotiationResponse.StatusCode == HttpStatusCode.NotFound)
                return ApiResponse.FromError<(FullQuizDto, ISyncContext)>(ApiErrors.QuizNotFound);

            var quizNegotiation = await quizNegotiationResponse.Content.ReadFromJsonAsync<QuizNegotiationDto>();
            if (quizNegotiation is null)
                return ApiResponse.FromError<(FullQuizDto, ISyncContext)>(ApiErrors.QuizNotFound);

            if (quizNegotiation.State == QuizState.InDevelopment && !quizNegotiation.CanEdit)
                return ApiResponse.FromError<(FullQuizDto, ISyncContext)>(ApiErrors.QuizNotOpen);

            var hubUrl = NavigationManager.ToAbsoluteUri($"Api/Quibble/{id}");
            var hubConnection =
                new HubConnectionBuilder()
                    .WithUrl(hubUrl)
                    .WithAutomaticReconnect()
                    .Build();

            await hubConnection.StartAsync();
            var getQuizHubResponse = await hubConnection.InvokeAsync<ApiResponse<FullQuizDto>>(SignalrEndpoints.GetQuiz);
            if (!getQuizHubResponse.WasSuccessful)
                return ApiResponse.FromError<(FullQuizDto, ISyncContext)>(getQuizHubResponse.Error);
            if (getQuizHubResponse.Value is null)
                return ApiResponse.FromError<(FullQuizDto, ISyncContext)>(ApiErrors.QuizNotFound);

            var quizDto = getQuizHubResponse.Value;
            var context = new SignalrSyncContext(LoggerFactory, hubConnection);

            return ApiResponse.FromSuccess<(FullQuizDto, ISyncContext)>((quizDto, context));
        }
    }
}
