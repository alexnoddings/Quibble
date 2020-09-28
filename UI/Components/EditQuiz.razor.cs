using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Quibble.UI.Core.Entities;

namespace Quibble.UI.Components
{
    public sealed partial class EditQuiz : IDisposable
    {
        [Parameter]
        public SyncedQuiz Quiz { get; set; } = default!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            if (Quiz == null) throw new ArgumentException(nameof(Quiz));

            await base.OnInitializedAsync();

            Quiz.Updated += OnQuizUpdatedAsync;
            Quiz.Deleted += OnQuizDeletedAsync;
        }

        private Task OnQuizUpdatedAsync() => InvokeAsync(StateHasChanged);

        private Task OnQuizDeletedAsync()
        {
            NavigationManager.NavigateTo("");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Quiz.Updated -= OnQuizUpdatedAsync;
            Quiz.Deleted -= OnQuizDeletedAsync;
        }
    }
}
