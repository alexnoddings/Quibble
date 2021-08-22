using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Entities;
using Quibble.Client.Sync.SignalR.Entities.EditMode;
using Quibble.Client.Sync.SignalR.Entities.HostMode;
using Quibble.Client.Sync.SignalR.Entities.TakeMode;
using Quibble.Shared.Api;
using Quibble.Shared.Entities;
using Quibble.Shared.Hub;
using Quibble.Shared.Models.Dtos;

namespace Quibble.Client.Sync.SignalR.Entities
{
    internal class SignalrSyncedQuizFactory : ISynchronisedQuizFactory
    {
        private ILogger<SignalrSyncedEntity> EntityLogger { get; }
        private HttpClient HttpClient { get; }
        private NavigationManager NavigationManager { get; }
        
        public SignalrSyncedQuizFactory(ILogger<SignalrSyncedEntity> entityLogger, IHttpClientFactory httpClientFactory, NavigationManager navigationManager)
        {
            EntityLogger = entityLogger;
            HttpClient = httpClientFactory.CreateClient("QuizApi");
            NavigationManager = navigationManager;
        }

        public async Task<ApiResponse<ISynchronisedQuiz>> GetQuizAsync(Guid quizId)
        {
            if (quizId == Guid.Empty)
                return ApiResponse.FromError<ISynchronisedQuiz>(HubErrors.QuizNotFound);

            var quizNegotiationResponse = await HttpClient.GetAsync($"{quizId}/negotiate");
            if (quizNegotiationResponse.StatusCode == HttpStatusCode.NotFound)
                return ApiResponse.FromError<ISynchronisedQuiz>(HubErrors.QuizNotFound);

            var quizNegotiation = await quizNegotiationResponse.Content.ReadFromJsonAsync<QuizNegotiationDto>();
            if (quizNegotiation is null)
                return ApiResponse.FromError<ISynchronisedQuiz>(HubErrors.QuizNotFound);

            if (quizNegotiation.State == QuizState.InDevelopment && !quizNegotiation.CanEdit)
                return ApiResponse.FromError<ISynchronisedQuiz>(HubErrors.QuizNotOpen);

            var hubUrl = NavigationManager.ToAbsoluteUri($"Api/Quibble/{quizId}");
            var hubConnection =
                new HubConnectionBuilder()
                    .WithUrl(hubUrl)
                    .WithAutomaticReconnect()
                    .Build();

            await hubConnection.StartAsync();
            var getQuizHubResponse = await hubConnection.InvokeAsync<ApiResponse<FullQuizDto>>(Endpoints.GetQuiz);
            if (!getQuizHubResponse.WasSuccessful)
                return ApiResponse.FromError<ISynchronisedQuiz>(getQuizHubResponse.Error);
            if (getQuizHubResponse.Value is null)
                return ApiResponse.FromError<ISynchronisedQuiz>(HubErrors.QuizNotFound);

            var quizDto = getQuizHubResponse.Value;
            ISynchronisedQuiz synchronisedQuiz;
            switch (quizNegotiation.State)
            {
                case QuizState.InDevelopment:
                    synchronisedQuiz =
                        new SignalrSyncedEditModeQuizBuilder()
                            .WithLoggerInstance(EntityLogger)
                            .WithHubConnection(hubConnection)
                            .WithQuiz(quizDto.Quiz)
                            .WithRounds(quizDto.Rounds)
                            .WithQuestions(quizDto.Questions)
                            .Build();
                    break;
                case QuizState.Open when quizNegotiation.CanEdit:
                    synchronisedQuiz =
                        new SignalrSyncedHostModeQuizBuilder()
                            .WithLoggerInstance(EntityLogger)
                            .WithHubConnection(hubConnection)
                            .WithQuiz(quizDto.Quiz)
                            .WithParticipants(quizDto.Participants)
                            .WithRounds(quizDto.Rounds)
                            .WithQuestions(quizDto.Questions)
                            .WithSubmittedAnswers(quizDto.SubmittedAnswers)
                            .Build();
                    break;
                case QuizState.Open:
                    synchronisedQuiz =
                        new SignalrSyncedTakeModeQuizBuilder()
                            .WithLoggerInstance(EntityLogger)
                            .WithHubConnection(hubConnection)
                            .WithQuiz(quizDto.Quiz)
                            .WithParticipants(quizDto.Participants)
                            .WithRounds(quizDto.Rounds)
                            .WithQuestions(quizDto.Questions)
                            .WithSubmittedAnswers(quizDto.SubmittedAnswers)
                            .Build();
                    break;
                default:
                    return ApiResponse.FromError<ISynchronisedQuiz>(HubErrors.UnknownError);
            }

            return ApiResponse.FromSuccess(synchronisedQuiz);
        }
    }
}
