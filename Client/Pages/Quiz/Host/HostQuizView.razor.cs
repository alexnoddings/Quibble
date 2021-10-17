using Microsoft.AspNetCore.Components;
using Quibble.Client.Sync;
using Quibble.Client.Sync.Core;

namespace Quibble.Client.Pages.Quiz.Host
{
    public sealed partial class HostQuizView : IDisposable
    {
        [Parameter]
        public ISyncedQuiz Quiz { get; set; } = default!;

        private SelectionContext Selection { get; set; } = default!;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Selection = new SelectionContext(Quiz);
            Quiz.Updated += OnUpdatedAsync;
            Selection.OnUpdated += OnUpdatedAsync;
        }

        private Task OnUpdatedAsync(SelectionChangedEventArgs _) => OnUpdatedAsync();

        protected override int CalculateStateStamp() =>
            StateStamp.ForProperties(Quiz, Selection.RoundNumber, Selection.QuestionNumber);

        public void Dispose()
        {
            Quiz.Updated -= OnUpdatedAsync;
            Selection.OnUpdated -= OnUpdatedAsync;
        }
    }
}
