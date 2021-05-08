using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Quibble.Client.Sync.Entities.EditMode;
using Quibble.Client.Sync.Entities;
using Quibble.Client.Sync;
using Quibble.Shared.Hub;

namespace Quibble.Client.Pages.Quiz
{
    [Authorize]
    public partial class Get : IAsyncDisposable
    {
        [Parameter]
        public Guid QuizId { get; set; }

        [Inject]
        private ISynchronisedQuizFactory QuizFactory { get; set; } = default!;

        private HubResponse<ISynchronisedQuiz>? GetQuizResult { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await ReloadQuizAsync();
        }

        private Task OnQuizInvalidated() =>
            ReloadQuizAsync();

        private async Task ReloadQuizAsync()
        {
            await DisposeQuizAsync();

            GetQuizResult = await QuizFactory.GetQuizAsync(QuizId);

            if (GetQuizResult.Value is ISynchronisedEditModeQuiz editModeQuiz)
                editModeQuiz.OnInvalidated += OnQuizInvalidated;

            await InvokeAsync(StateHasChanged);
        }

        private async Task DisposeQuizAsync()
        {
            var syncedQuiz = GetQuizResult?.Value;
            if (syncedQuiz is null) return;

            if (syncedQuiz is ISynchronisedEditModeQuiz editModeQuiz)
                editModeQuiz.OnInvalidated -= OnQuizInvalidated;

            await syncedQuiz.DisposeAsync();
            GetQuizResult = null;
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeQuizAsync();
        }
    }
}
