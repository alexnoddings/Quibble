using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Entities;
using Quibble.Client.Sync.Internal.EditMode;
using Quibble.Client.Sync.Internal.HostMode;
using Quibble.Client.Sync.Internal.TakeMode;
using Quibble.Shared.Entities;
using Quibble.Shared.Hub;
using Quibble.Shared.Models.Dtos;

namespace Quibble.Client.Sync.Internal
{
    internal class SynchronisedQuizFactory : ISynchronisedQuizFactory
    {
        private ILogger<SignalrSynchronisedEntity> EntityLogger { get; }
        private HttpClient HttpClient { get; }
        private NavigationManager NavigationManager { get; }
        
        public SynchronisedQuizFactory(ILogger<SignalrSynchronisedEntity> entityLogger, IHttpClientFactory httpClientFactory, NavigationManager navigationManager)
        {
            EntityLogger = entityLogger;
            HttpClient = httpClientFactory.CreateClient("QuizApi");
            NavigationManager = navigationManager;
        }

        public async Task<HubResponse<ISynchronisedQuiz>> GetQuizAsync(Guid quizId)
        {
            if (quizId == Guid.Empty)
                return HubResponse.FromError<ISynchronisedQuiz>(HubErrors.QuizNotFound);

            var quizNegotiationResponse = await HttpClient.GetAsync($"{quizId}/negotiate");
            if (quizNegotiationResponse.StatusCode == HttpStatusCode.NotFound)
                return HubResponse.FromError<ISynchronisedQuiz>(HubErrors.QuizNotFound);

            var quizNegotiation = await quizNegotiationResponse.Content.ReadFromJsonAsync<QuizNegotiationDto>();
            if (quizNegotiation is null)
                return HubResponse.FromError<ISynchronisedQuiz>(HubErrors.QuizNotFound);

            if (quizNegotiation.State == QuizState.InDevelopment && !quizNegotiation.CanEdit)
                return HubResponse.FromError<ISynchronisedQuiz>(HubErrors.QuizNotOpen);

            var hubUrl = NavigationManager.ToAbsoluteUri($"Api/Quibble/{quizId}");
            var hubConnection =
                new HubConnectionBuilder()
                    .WithUrl(hubUrl)
                    .WithAutomaticReconnect()
                    .Build();

            await hubConnection.StartAsync();
            var getQuizHubResponse = await hubConnection.InvokeAsync<HubResponse<FullQuizDto>>(Endpoints.GetQuiz);
            if (!getQuizHubResponse.WasSuccessful)
                return HubResponse.FromError<ISynchronisedQuiz>(getQuizHubResponse.Error);
            if (getQuizHubResponse.Value is null)
                return HubResponse.FromError<ISynchronisedQuiz>(HubErrors.QuizNotFound);

            var quizDto = getQuizHubResponse.Value;
            ISynchronisedQuiz synchronisedQuiz;
            switch (quizNegotiation.State)
            {
                case QuizState.InDevelopment:
                    synchronisedQuiz =
                        new SynchronisedEditModeQuizBuilder()
                            .WithLoggerInstance(EntityLogger)
                            .WithHubConnection(hubConnection)
                            .WithQuiz(quizDto.Quiz)
                            .WithRounds(quizDto.Rounds)
                            .WithQuestions(quizDto.Questions)
                            .Build();
                    break;
                case QuizState.Open when quizNegotiation.CanEdit:
                    synchronisedQuiz =
                        new SynchronisedHostModeQuizBuilder()
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
                        new SynchronisedTakeModeQuizBuilder()
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
                    return HubResponse.FromError<ISynchronisedQuiz>(HubErrors.UnknownError);
            }

            return HubResponse.FromSuccess(synchronisedQuiz);
        }
    }
}
