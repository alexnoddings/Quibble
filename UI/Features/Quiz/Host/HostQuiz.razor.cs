using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Quibble.UI.Core.Entities;

namespace Quibble.UI.Features.Quiz.Host
{
    public sealed partial class HostQuiz : IDisposable
    {
        [CascadingParameter]
        public SyncedQuiz Quiz { get; set; } = default!;

        private int SelectedRoundNumber { get; set; } = 0;
        private int SelectedQuestionNumber { get; set; } = 0;

        private SyncedRound SelectedRound => Quiz.Rounds[SelectedRoundNumber];
        private SyncedQuestion SelectedQuestion => SelectedRound.Questions[SelectedQuestionNumber];

        protected override async Task OnInitializedAsync()
        {
            if (Quiz == null) throw new ArgumentException(nameof(Quiz));

            await base.OnInitializedAsync();

            Quiz.Updated += OnQuizUpdatedAsync;
        }

        private Task OnQuizUpdatedAsync() => InvokeAsync(StateHasChanged);

        private void MoveToPreviousQuestion()
        {
            if (SelectedQuestionNumber > 0)
            {
                SelectedQuestionNumber--;
            }
            else if (SelectedRoundNumber > 0)
            {
                SelectedRoundNumber--;
                SelectedQuestionNumber = SelectedRound.Questions.Count - 1;
            }
        }

        private void MoveToNextQuestion()
        {
            if (SelectedQuestionNumber < SelectedRound.Questions.Count - 1)
            {
                SelectedQuestionNumber++;
            }
            else if (SelectedRoundNumber < Quiz.Rounds.Count - 1)
            {
                SelectedQuestionNumber = 0;
                SelectedRoundNumber++;
            }
        }

        public void Dispose()
        {
            if (Quiz != null)
                Quiz.Updated -= OnQuizUpdatedAsync;
        }
    }
}
