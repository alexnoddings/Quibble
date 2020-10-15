using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Quibble.UI.Core.Entities;

namespace Quibble.UI.Features.Quiz.Host
{
    public sealed partial class HostQuizQuestionSelect : IDisposable
    {

        [Parameter]
        public SyncedQuiz Quiz { get; set; } = default!;

        [Parameter]
        public int SelectedRoundNumber { get; set; } = 0;

        [Parameter]
        public int SelectedQuestionNumber { get; set; } = 0;

        [Parameter]
        public Action<int, int> SelectQuestion { get; set; } = default!;

        private int AccordionSelectedRound { get; set; } = -1;

        protected override async Task OnInitializedAsync()
        {
            if (Quiz == null) throw new ArgumentException(nameof(Quiz));
            if (SelectQuestion == null) throw new ArgumentException(nameof(SelectQuestion));

            await base.OnInitializedAsync();

            Quiz.Updated += OnQuizUpdatedAsync;
        }

        private Task OnQuizUpdatedAsync() => InvokeAsync(StateHasChanged);

        private void SelectAccordionRound(int roundNumber)
        {
            if (AccordionSelectedRound == roundNumber)
                AccordionSelectedRound = -1;
            else
                AccordionSelectedRound = roundNumber;
        }

        public void Dispose()
        {
            if (Quiz != null)
                Quiz.Updated -= OnQuizUpdatedAsync;
        }
    }
}
