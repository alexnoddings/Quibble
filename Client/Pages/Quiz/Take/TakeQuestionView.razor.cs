using Microsoft.AspNetCore.Components;
using Quibble.Client.Sync;
using Quibble.Client.Sync.Core.Entities;
using Quibble.Client.Sync.Extensions;

namespace Quibble.Client.Pages.Quiz.Take;

public sealed partial class TakeQuestionView : IDisposable
{
    [Parameter]
    public ISyncedQuestion Question { get; set; } = default!;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Question.Updated += OnUpdatedAsync;
        var answer = Question.TryGetMyAnswer();
        if (answer is not null)
            answer.Updated += OnUpdatedAsync;
    }

    protected override int CalculateStateStamp() =>
        StateStamp.ForProperties(Question);

    public void Dispose()
    {
        Question.Updated -= OnUpdatedAsync;
        var answer = Question.TryGetMyAnswer();
        if (answer is not null)
            answer.Updated -= OnUpdatedAsync;
    }
}
