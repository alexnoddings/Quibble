using Microsoft.AspNetCore.Components;
using Quibble.Client.Sync.Entities.HostMode;
using Quibble.Shared.Entities;

namespace Quibble.Client.Pages.Quiz.Host
{
    public sealed partial class HostQuestionView : IDisposable
    {
        [Parameter]
        public ISynchronisedHostModeQuiz Quiz { get; set; } = default!;

        [Parameter]
        public SelectionContext Selection { get; set; } = default!;

        private ISynchronisedHostModeQuestion LastQuestion { get; set; } = default!;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            LastQuestion = Selection.Question;
            LastQuestion.Round.Updated += OnUpdatedAsync;
            LastQuestion.Updated += OnUpdatedAsync;
            foreach (var answer in LastQuestion.Answers)
                answer.Updated += OnUpdatedAsync;

            Selection.Updated += OnQuestionChangedAsync;
        }

        private Task OnUpdatedAsync() => InvokeAsync(StateHasChanged);

        private Task OnQuestionChangedAsync()
        {
            LastQuestion.Round.Updated -= OnUpdatedAsync;
            LastQuestion.Updated -= OnUpdatedAsync;
            foreach (var answer in LastQuestion.Answers)
                answer.Updated -= OnUpdatedAsync;

            LastQuestion = Selection.Question;

            LastQuestion.Round.Updated += OnUpdatedAsync;
            LastQuestion.Updated += OnUpdatedAsync;
            foreach (var answer in LastQuestion.Answers)
                answer.Updated += OnUpdatedAsync;

            return OnUpdatedAsync();
        }

        private async Task ShowAllRoundsAsync()
        {
            foreach (var round in Quiz.Rounds)
                if (round.State == RoundState.Hidden)
                    await round.OpenAsync();
        }

        private async Task ShowAllQuestionsInRoundAsync()
        {
            foreach (var question in Selection.Round.Questions)
                if (question.State == QuestionState.Hidden)
                    await question.OpenAsync();
        }

        private async Task LockAllQuestionsInRoundAsync()
        {
            foreach (var question in Selection.Round.Questions)
                if (question.State == QuestionState.Open)
                    await question.LockAsync();
        }

        private async Task ShowAllQuestionAnswersInRoundAsync()
        {
            foreach (var question in Selection.Round.Questions)
                if (question.State == QuestionState.Locked && question.Answers.All(answer => answer.AssignedPoints >= 0))
                    await question.ShowAnswer();
        }

        public void Dispose()
        {
            LastQuestion.Round.Updated -= OnUpdatedAsync;
            LastQuestion.Updated -= OnUpdatedAsync;
            foreach (var answer in LastQuestion.Answers)
                answer.Updated += OnUpdatedAsync;

            Selection.Updated -= OnQuestionChangedAsync;
        }
    }
}
