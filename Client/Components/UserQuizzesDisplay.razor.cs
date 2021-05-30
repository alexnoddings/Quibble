using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Quibble.Shared.Models;

namespace Quibble.Client.Components
{
    public partial class UserQuizzesDisplay
    {
        [Inject]
        private IHttpClientFactory HttpClientFactory { get; set; } = default!;

        private UserQuizzes? UserQuizzes { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            var httpClient = HttpClientFactory.CreateClient("QuizApi");
            UserQuizzes = await httpClient.GetFromJsonAsync<UserQuizzes>("");
        }
    }
}
