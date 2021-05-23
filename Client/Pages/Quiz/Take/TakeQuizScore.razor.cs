using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Quibble.Client.Sync.Entities.TakeMode;

namespace Quibble.Client.Pages.Quiz.Take
{
    public sealed partial class TakeQuizScore : IDisposable
    {
        [Parameter]
        public ISynchronisedTakeModeQuiz Quiz { get; set; } = default!;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Quiz.Updated += OnUpdatedAsync;
            Quiz.RoundAdded += OnRoundAddedAsync;
            foreach (var round in Quiz.Rounds)
            {
                round.Updated += OnUpdatedAsync;
                round.QuestionAdded += OnQuestionAddedAsync;
                foreach (var question in round.Questions)
                {
                    question.Updated += OnUpdatedAsync;
                    if (question.SubmittedAnswer is not null)
                        question.SubmittedAnswer.Updated += OnUpdatedAsync;
                }
            }
        }

        private Task OnUpdatedAsync() => InvokeAsync(StateHasChanged);

        private Task OnRoundAddedAsync(ISynchronisedTakeModeRound round)
        {
            round.Updated += OnUpdatedAsync;
            round.QuestionAdded += OnQuestionAddedAsync;
            return Task.CompletedTask;
        }

        private Task OnQuestionAddedAsync(ISynchronisedTakeModeQuestion question)
        {
            question.Updated += OnUpdatedAsync;
            Console.WriteLine($"TQS: OQA: {question.SubmittedAnswer?.AssignedPoints}");
            if (question.SubmittedAnswer is not null)
                question.SubmittedAnswer.Updated += OnUpdatedAsync;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Quiz.Updated -= OnUpdatedAsync;
            Quiz.RoundAdded -= OnRoundAddedAsync;
            foreach (var round in Quiz.Rounds)
            {
                round.Updated -= OnUpdatedAsync;
                round.QuestionAdded -= OnQuestionAddedAsync;
                foreach (var question in round.Questions)
                {
                    question.Updated -= OnUpdatedAsync;
                    if (question.SubmittedAnswer is not null)
                        question.SubmittedAnswer.Updated -= OnUpdatedAsync;
                }
            }
        }
    }
}
