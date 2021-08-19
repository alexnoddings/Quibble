using Microsoft.AspNetCore.Components;
using Quibble.Client.Sync.Entities.HostMode;

namespace Quibble.Client.Pages.Quiz.Host
{
    public sealed partial class HostQuizView : IDisposable
    {
        [Parameter]
        public ISynchronisedHostModeQuiz Quiz { get; set; } = default!;

        private SelectionContext Selection { get; set; } = default!;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Selection = new SelectionContext(Quiz);
            Quiz.Updated += OnUpdatedAsync;
            Selection.Updated += OnUpdatedAsync;
        }

        private Task OnUpdatedAsync() => InvokeAsync(StateHasChanged);

        public void Dispose()
        {
            Quiz.Updated -= OnUpdatedAsync;
            Selection.Updated -= OnUpdatedAsync;
        }
    }
}
