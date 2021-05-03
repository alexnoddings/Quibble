using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Quibble.Client.Sync.Entities;
using Quibble.Client.Sync.Internal.EditMode;
using Quibble.Shared.Entities;
using Quibble.Shared.Hub;
using Quibble.Shared.Models;
using Quibble.Shared.Resources;

namespace Quibble.Client.Sync.Internal
{
    internal class SynchronisedQuizFactory : ISynchronisedQuizFactory
    {
        private HttpClient HttpClient { get; }
        private NavigationManager NavigationManager { get; }

        public SynchronisedQuizFactory(IHttpClientFactory httpClientFactory, NavigationManager navigationManager)
        {
            HttpClient = httpClientFactory.CreateClient("QuizApi");
            NavigationManager = navigationManager;
        }

        public async Task<HubResponse<ISynchronisedEntity>> GetQuizAsync(Guid quizId)
        {
            if (quizId == Guid.Empty)
                return HubResponse.FromError<ISynchronisedEntity>(nameof(ErrorMessages.QuizNotFound));

            var quizNegotiationResponse = await HttpClient.GetAsync($"{quizId}/negotiate");
            var quizNegotiation = await quizNegotiationResponse.Content.ReadFromJsonAsync<QuizNegotiationDto>();
            if (quizNegotiation is null)
                return HubResponse.FromError<ISynchronisedEntity>(nameof(ErrorMessages.QuizNotFound));

            if (quizNegotiation.State == QuizState.InDevelopment && !quizNegotiation.CanEdit)
                    return HubResponse.FromError<ISynchronisedEntity>(nameof(ErrorMessages.QuizNotOpen));

            var hubUrl = NavigationManager.ToAbsoluteUri($"Api/Quibble/{quizId}");
            var hubConnection =
                new HubConnectionBuilder()
                    .WithUrl(hubUrl)
                    .WithAutomaticReconnect()
                    .Build();

            await hubConnection.StartAsync();
            var getQuizHubResponse = await hubConnection.InvokeAsync<HubResponse<FullQuizDto>>(Endpoints.GetQuiz);
            if (!getQuizHubResponse.WasSuccessful)
                return HubResponse.FromError<ISynchronisedEntity>(getQuizHubResponse.ErrorCode);
            if (getQuizHubResponse.Value is null)
                return HubResponse.FromError<ISynchronisedEntity>(nameof(ErrorMessages.QuizNotFound));

            var quiz = getQuizHubResponse.Value.Quiz;
            var rounds = getQuizHubResponse.Value.Rounds;
            var questions = getQuizHubResponse.Value.Questions;

            ISynchronisedEntity synchronisedEntity;
            if (quizNegotiation.State == QuizState.InDevelopment)
            {
                synchronisedEntity =
                    new SynchronisedEditModeQuizBuilder()
                        .WithHubConnection(hubConnection)
                        .WithQuiz(quiz)
                        .WithRounds(rounds)
                        .WithQuestions(questions)
                        .Build();
            }
            else
            {
                // ToDo: add when other modes are added
                throw new NotImplementedException();
            }

            return HubResponse.FromSuccess(synchronisedEntity);
        }
    }
}
