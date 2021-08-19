using Microsoft.AspNetCore.Components;
using Quibble.Client.Sync.Entities.HostMode;

namespace Quibble.Client.Pages.Quiz.Host
{
    public sealed partial class HostQuestionList : IDisposable
    {
        [Parameter]
        public ISynchronisedHostModeQuiz Quiz { get; set; } = default!;

        [Parameter]
        public SelectionContext Selection { get; set; } = default!;

        private int HighlightedRoundNumber { get; set; }

        protected override void OnInitialized()
        {
            Selection.Updated += OnSelectionUpdatedAsync;
            foreach (var round in Quiz.Rounds)
            {
                round.Updated += OnUpdatedAsync;
                foreach (var question in round.Questions)
                    question.Updated += OnUpdatedAsync;
            }
        }

        private Task OnUpdatedAsync() => InvokeAsync(StateHasChanged);
        private Task OnSelectionUpdatedAsync()
        {
            HighlightedRoundNumber = Selection.RoundNumber;
            return OnUpdatedAsync();
        }

        public void Dispose()
        {
            Selection.Updated -= OnSelectionUpdatedAsync;
            foreach (var round in Quiz.Rounds)
            {
                round.Updated -= OnUpdatedAsync;
                foreach (var question in round.Questions)
                    question.Updated -= OnUpdatedAsync;
            }
        }
    }
}
