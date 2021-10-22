using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Quibble.Client.Components.Modals;
using Quibble.Client.Sync;
using Quibble.Client.Sync.Core.Entities;

namespace Quibble.Client.Pages.Quiz.Edit;

public sealed partial class EditRoundView : IDisposable
{
    [Parameter]
    public ISyncedRound Round { get; set; } = default!;

    private OptionsModal<bool> ConfirmDeleteModal { get; set; } = default!;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Round.Updated += OnUpdatedAsync;
    }

    private async Task OnDeleteClickedAsync(MouseEventArgs args)
    {
        if (args.ShiftKey
            || !Round.Questions.Any()
            || Round.Questions.All(question => string.IsNullOrWhiteSpace(question.Text) && string.IsNullOrWhiteSpace(question.Answer))
            || await ConfirmDeleteModal.ShowAsync(false))
        {
            await Round.DeleteAsync();
        }
    }

    private async Task AddQuestionsAsync(int count)
    {
        for (var i = 0; i < count; i++)
            await Round.AddQuestionAsync();
    }

    protected override int CalculateStateStamp() =>
        StateStamp.ForProperties(Round);

    public void Dispose()
    {
        Round.Updated -= OnUpdatedAsync;
    }
}
