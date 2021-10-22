using Microsoft.AspNetCore.Components;
using Quibble.Client.Sync;
using Quibble.Client.Sync.Core.Entities;

namespace Quibble.Client.Pages.Quiz.Take;

public sealed partial class TakeQuizView : IDisposable
{
    [Parameter]
    public ISyncedQuiz Quiz { get; set; } = default!;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Quiz.Updated += OnUpdatedAsync;
    }

    protected override int CalculateStateStamp() =>
        StateStamp.ForProperties(Quiz);

    public void Dispose()
    {
        Quiz.Updated -= OnUpdatedAsync;
    }
}
