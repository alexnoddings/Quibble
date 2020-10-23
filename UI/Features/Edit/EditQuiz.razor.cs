using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Quibble.UI.Core.Entities;

namespace Quibble.UI.Features.Edit
{
    public sealed partial class EditQuiz : IDisposable
    {
        [CascadingParameter]
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

        private async Task AddRoundsAsync(int count)
        {
            for (var i = 0; i < count; i++)
            {
                await Quiz.AddRoundAsync();
            }
        }

        public void Dispose()
        {
            Quiz.Updated -= OnQuizUpdatedAsync;
            Quiz.Deleted -= OnQuizDeletedAsync;
        }
    }
}
