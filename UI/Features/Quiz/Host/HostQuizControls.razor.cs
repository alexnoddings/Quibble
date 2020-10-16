using Microsoft.AspNetCore.Components;
using Quibble.Core.Entities;
using Quibble.UI.Core.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Quibble.UI.Features.Quiz.Host
{
    public sealed partial class HostQuizControls : IDisposable
    {

        [CascadingParameter]
        public SyncedQuiz Quiz { get; set; } = default!;

        [Parameter]
        public int SelectedRoundNumber { get; set; } = 0;

        [Parameter]
        public int SelectedQuestionNumber { get; set; } = 0;

        [Parameter]
        public Action MoveToPreviousQuestion { get; set; } = default!;

        [Parameter]
        public Action MoveToNextQuestion { get; set; } = default!;

        private SyncedRound SelectedRound => Quiz.Rounds[SelectedRoundNumber];
        private SyncedQuestion SelectedQuestion => SelectedRound.Questions[SelectedQuestionNumber];

        protected override async Task OnInitializedAsync()
        {
            if (Quiz == null) throw new ArgumentException(nameof(Quiz));
            if (MoveToPreviousQuestion == null) throw new ArgumentException(nameof(MoveToPreviousQuestion));
            if (MoveToNextQuestion == null) throw new ArgumentException(nameof(MoveToNextQuestion));

            await base.OnInitializedAsync();

            Quiz.Updated += OnQuizUpdatedAsync;

        }

        private Task OnQuizUpdatedAsync() => InvokeAsync(StateHasChanged);

        private Task RevealRoundAsync() => SelectedRound.UpdateStateAsync(RoundState.Visible);

        private Task RevealQuestionAsync() => SelectedQuestion.UpdateStateAsync(QuestionState.Visible);

        private Task LockQuestionAsync() => SelectedQuestion.UpdateStateAsync(QuestionState.Locked);

        private Task RevealCorrectAnswerAsync() => SelectedQuestion.UpdateStateAsync(QuestionState.AnswerRevealed);

        private async Task RevealRoundQuestionsAsync()
        {
            foreach (var question in SelectedRound.Questions.Where(q => q.State == QuestionState.Hidden))
                await question.UpdateStateAsync(QuestionState.Visible);
        }

        private async Task LockRoundQuestionsAsync()
        {
            foreach (var question in SelectedRound.Questions.Where(q => q.State == QuestionState.Visible))
                await question.UpdateStateAsync(QuestionState.Locked);
        }

        private async Task RevealRoundAnswersAsync()
        {
            foreach (var question in SelectedRound.Questions.Where(q => q.State == QuestionState.Locked))
                await question.UpdateStateAsync(QuestionState.AnswerRevealed);
        }

        public void Dispose()
        {
            if (Quiz != null)
                Quiz.Updated -= OnQuizUpdatedAsync;
        }
    }
}
