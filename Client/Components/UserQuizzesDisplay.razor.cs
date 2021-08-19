using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Quibble.Shared.Models;

namespace Quibble.Client.Components
{
    public partial class UserQuizzesDisplay
    {

        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        private IHttpClientFactory HttpClientFactory { get; set; } = default!;

        private HttpClient HttpClient { get; set; } = default!;

        private UserQuizzes? UserQuizzes { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            HttpClient = HttpClientFactory.CreateClient("QuizApi");
            UserQuizzes = await HttpClient.GetFromJsonAsync<UserQuizzes>("");
        }

        private async Task CreateNewQuizAsync()
        {
            var response = await HttpClient.PostAsync("", null);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<Guid?>();
            if (!result.HasValue)
                throw new InvalidOperationException();

            var url = $"/quiz/{result}";
            NavigationManager.NavigateTo(url);
        }
    }
}
