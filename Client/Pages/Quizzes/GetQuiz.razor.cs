using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Quibble.Common.Quizzes;

namespace Quibble.Client.Pages.Quizzes
{
    public partial class GetQuiz : IAsyncDisposable
    {
        [Parameter]
        public Guid Id { get; set; }

        private QuizFull? Quiz { get; set; }

        private bool WasDeleted { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);

            QuizHubConnection.OnQuizUpdated(OnQuizUpdatedAsync);
            QuizHubConnection.OnQuizDeleted(OnQuizDeletedAsync);

            await QuizHubConnection.StartAsync().ConfigureAwait(false);

            Quiz = await QuizHubConnection.GetFullAsync(Id).ConfigureAwait(false);

            if (Quiz == null) throw new InvalidOperationException();

            await QuizHubConnection.RegisterForUpdatesAsync(Id).ConfigureAwait(false);
        }

        private Task OnQuizUpdatedAsync(Quiz quiz)
        {
            if (Quiz == null || quiz.State == Quiz.State) return Task.CompletedTask;

            Quiz.State = quiz.State;
            return StateHasChangedAsync();
        }

        private Task OnQuizDeletedAsync(Guid id)
        {
            Quiz = null;
            WasDeleted = true;
            return Task.CompletedTask;
        }

        private Task StateHasChangedAsync() => InvokeAsync(StateHasChanged);

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            if (QuizHubConnection != null)
                await QuizHubConnection.DisposeAsync().ConfigureAwait(false);
        }
    }
}
