using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Quibble.Client.Components.Modals;
using Quibble.Client.Sync;
using Quibble.Client.Sync.Core.Entities;

namespace Quibble.Client.Pages.Quiz.Edit;

public sealed partial class EditQuizView : IDisposable
{
    [Parameter]
    public ISyncedQuiz Quiz { get; set; } = default!;

    private OptionsModal<bool> ConfirmPublishModal { get; set; } = default!;

    private OptionsModal<bool> ConfirmDeleteModal { get; set; } = default!;

    private List<string> ErrorMessages { get; set; } = new();

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Quiz.Updated += OnUpdatedAsync;
    }

    private async Task OnDeleteClickedAsync(MouseEventArgs args)
    {
        if (!Quiz.Rounds.Any() || await ConfirmDeleteModal.ShowAsync(false))
        {
            await Quiz.DeleteAsync();
        }
    }

    private async Task AddRoundsAsync(int count)
    {
        for (var i = 0; i < count; i++)
            await Quiz.AddRoundAsync();
    }

    private async Task PublishAsync()
    {
        ErrorMessages.Clear();

        if (!Quiz.Rounds.SelectMany(round => round.Questions).Any())
        {
            ErrorMessages.Add("No questions present.");
            return;
        }

        var roundCount = 0;
        foreach (var round in Quiz.Rounds)
        {
            roundCount++;
            if (string.IsNullOrWhiteSpace(round.Title))
                ErrorMessages.Add($"Round #{roundCount} is missing a title.");

            var questionCount = 0;
            foreach (var question in round.Questions)
            {
                questionCount++;

                if (string.IsNullOrWhiteSpace(question.Text))
                    ErrorMessages.Add($"Question #{roundCount}.{questionCount} is missing text.");
                if (string.IsNullOrWhiteSpace(question.Answer))
                    ErrorMessages.Add($"Question #{roundCount}.{questionCount} is missing an answer.");
            }
        }

        if (await ConfirmPublishModal.ShowAsync(false))
            await Quiz.OpenAsync();
    }

    protected override int CalculateStateStamp() =>
        StateStamp.ForProperties(Quiz, ErrorMessages);

    public void Dispose()
    {
        Quiz.Updated -= OnUpdatedAsync;
    }
}
