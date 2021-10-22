using Microsoft.AspNetCore.Components;
using Quibble.Client.Sync;
using Quibble.Client.Sync.Core.Entities;
using Quibble.Client.Sync.Extensions;

namespace Quibble.Client.Pages.Quiz.Take;

public sealed partial class TakeQuizScore : IDisposable
{
    [Parameter]
    public ISyncedQuiz Quiz { get; set; } = default!;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Quiz.Updated += OnUpdatedAsync;
        Quiz.Rounds.Added += OnRoundAddedAsync;
        foreach (var round in Quiz.Rounds)
        {
            round.Updated += OnUpdatedAsync;
            round.Questions.Added += OnQuestionAddedAsync;
            foreach (var question in round.Questions)
            {
                question.Updated += OnUpdatedAsync;
                var answer = question.TryGetMyAnswer();
                if (answer is not null)
                    answer.Updated += OnUpdatedAsync;
            }
        }
    }

    private Task OnRoundAddedAsync(ISyncedRound round)
    {
        round.Updated += OnUpdatedAsync;
        round.Questions.Added += OnQuestionAddedAsync;
        return Task.CompletedTask;
    }

    private Task OnQuestionAddedAsync(ISyncedQuestion question)
    {
        question.Updated += OnUpdatedAsync;
        var answer = question.TryGetMyAnswer();
        if (answer is not null)
            answer.Updated += OnUpdatedAsync;
        return Task.CompletedTask;
    }

    protected override int CalculateStateStamp() =>
        StateStamp.ForProperties(Quiz);

    public void Dispose()
    {
        Quiz.Updated -= OnUpdatedAsync;
        Quiz.Rounds.Added -= OnRoundAddedAsync;
        foreach (var round in Quiz.Rounds)
        {
            round.Updated -= OnUpdatedAsync;
            round.Questions.Added -= OnQuestionAddedAsync;
            foreach (var question in round.Questions)
            {
                question.Updated -= OnUpdatedAsync;
                var answer = question.TryGetMyAnswer();
                if (answer is not null)
                    answer.Updated += OnUpdatedAsync;
            }
        }
    }
}
