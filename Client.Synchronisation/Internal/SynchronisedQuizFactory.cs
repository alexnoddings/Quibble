using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Entities;
using Quibble.Client.Sync.Internal.EditMode;
using Quibble.Client.Sync.Internal.HostMode;
using Quibble.Shared.Entities;
using Quibble.Shared.Hub;
using Quibble.Shared.Models;
using Quibble.Shared.Resources;

namespace Quibble.Client.Sync.Internal
{
    internal class SynchronisedQuizFactory : ISynchronisedQuizFactory
    {
        private ILogger<SynchronisedEntity> EntityLogger { get; }
        private HttpClient HttpClient { get; }
        private NavigationManager NavigationManager { get; }

        public SynchronisedQuizFactory(ILogger<SynchronisedEntity> entityLogger, IHttpClientFactory httpClientFactory, NavigationManager navigationManager)
        {
            EntityLogger = entityLogger;
            HttpClient = httpClientFactory.CreateClient("QuizApi");
            NavigationManager = navigationManager;
        }

        public async Task<HubResponse<ISynchronisedQuiz>> GetQuizAsync(Guid quizId)
        {
            if (quizId == Guid.Empty)
                return HubResponse.FromError<ISynchronisedQuiz>(nameof(ErrorMessages.QuizNotFound));

            var quizNegotiationResponse = await HttpClient.GetAsync($"{quizId}/negotiate");
            if (quizNegotiationResponse.StatusCode == HttpStatusCode.NotFound)
                return HubResponse.FromError<ISynchronisedQuiz>(nameof(ErrorMessages.QuizNotFound));

            var quizNegotiation = await quizNegotiationResponse.Content.ReadFromJsonAsync<QuizNegotiationDto>();
            if (quizNegotiation is null)
                return HubResponse.FromError<ISynchronisedQuiz>(nameof(ErrorMessages.QuizNotFound));

            if (quizNegotiation.State == QuizState.InDevelopment && !quizNegotiation.CanEdit)
                return HubResponse.FromError<ISynchronisedQuiz>(nameof(ErrorMessages.QuizNotOpen));

            var hubUrl = NavigationManager.ToAbsoluteUri($"Api/Quibble/{quizId}");
            var hubConnection =
                new HubConnectionBuilder()
                    .WithUrl(hubUrl)
                    .WithAutomaticReconnect()
                    .Build();

            await hubConnection.StartAsync();
            var getQuizHubResponse = await hubConnection.InvokeAsync<HubResponse<FullQuizDto>>(Endpoints.GetQuiz);
            if (!getQuizHubResponse.WasSuccessful)
                return HubResponse.FromError<ISynchronisedQuiz>(getQuizHubResponse.ErrorCode);
            if (getQuizHubResponse.Value is null)
                return HubResponse.FromError<ISynchronisedQuiz>(nameof(ErrorMessages.QuizNotFound));

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
                    // ToDo: implement
                    return HubResponse.FromError<ISynchronisedQuiz>("NotImplemented");
                default:
                    return HubResponse.FromError<ISynchronisedQuiz>(nameof(ErrorMessages.UnknownError));
            }

            return HubResponse.FromSuccess(synchronisedQuiz);
        }
    }
}
