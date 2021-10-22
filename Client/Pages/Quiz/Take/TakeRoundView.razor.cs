using Microsoft.AspNetCore.Components;
using Quibble.Client.Sync;
using Quibble.Client.Sync.Core.Entities;

namespace Quibble.Client.Pages.Quiz.Take;

public sealed partial class TakeRoundView : IDisposable
{
    [Parameter]
    public ISyncedRound Round { get; set; } = default!;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Round.Updated += OnUpdatedAsync;
        Round.Questions.Added += OnQuestionAddedAsync;
    }

    private Task OnQuestionAddedAsync(ISyncedQuestion question)
    {
        question.Updated += OnUpdatedAsync;
        foreach (var submittedAnswers in question.SubmittedAnswers)
            submittedAnswers.Updated += OnUpdatedAsync;

        return Task.CompletedTask;
    }

    protected override int CalculateStateStamp() =>
        StateStamp.ForProperties(Round);

    public void Dispose()
    {
        Round.Updated -= OnUpdatedAsync;
        foreach (var question in Round.Questions)
        {
            question.Updated -= OnUpdatedAsync;
            foreach (var submittedAnswers in question.SubmittedAnswers)
                submittedAnswers.Updated -= OnUpdatedAsync;
        }
    }
}
