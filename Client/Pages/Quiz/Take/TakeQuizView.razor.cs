using Microsoft.AspNetCore.Components;
using Quibble.Client.Sync.Entities.TakeMode;

namespace Quibble.Client.Pages.Quiz.Take
{
    public sealed partial class TakeQuizView : IDisposable
    {
        [Parameter]
        public ISyncedTakeModeQuiz Quiz { get; set; } = default!;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Quiz.Updated += OnUpdatedAsync;
        }

        private Task OnUpdatedAsync() => InvokeAsync(StateHasChanged);

        public void Dispose()
        {
            Quiz.Updated -= OnUpdatedAsync;
        }
    }
}
