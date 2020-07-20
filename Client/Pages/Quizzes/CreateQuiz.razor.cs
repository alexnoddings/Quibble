using System;
using System.Threading.Tasks;
using Quibble.Common.Quizzes;

namespace Quibble.Client.Pages.Quizzes
{
    public partial class CreateQuiz : IAsyncDisposable
    {
        private string NewQuizTitle { get; set; } = string.Empty;

        private string? ErrorDetail { get; set; } = null;

        protected override async Task OnInitializedAsync()
        {
            await QuizHubConnection.StartAsync().ConfigureAwait(false);
        }

        private async Task CreateQuizAsync()
        {
            var newQuiz = new Quiz {Title = NewQuizTitle};
            var quiz = await QuizHubConnection.CreateAsync(newQuiz).ConfigureAwait(false);

            NavigationManager.NavigateTo($"/quiz/{quiz.Id}/edit");
        }

        public async ValueTask DisposeAsync()
        {
            if (QuizHubConnection != null)
                await QuizHubConnection.DisposeAsync().ConfigureAwait(false);
        }
    }
}
