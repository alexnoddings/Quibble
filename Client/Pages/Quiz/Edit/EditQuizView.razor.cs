using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Quibble.Client.Components.Modals;
using Quibble.Client.Sync.Entities.EditMode;

namespace Quibble.Client.Pages.Quiz.Edit
{
    public partial class EditQuizView : IDisposable
    {
        [Parameter]
        public ISynchronisedEditModeQuiz Quiz { get; set; } = default!;

        private OptionsModal<bool> ConfirmPublishModal { get; set; } = default!;

        private OptionsModal<bool> ConfirmDeleteModal { get; set; } = default!;

        private Alert ErrorMessagesAlert { get; set; } = default!;

        private List<string>? ErrorMessages { get; set; }

        private int LastStateStamp { get; set; } = 0;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Quiz.Updated += OnUpdatedAsync;
            LastStateStamp = Quiz.GetStateStamp();
        }

        private Task OnUpdatedAsync() => InvokeAsync(StateHasChanged);

        private async Task OnDeleteClickedAsync(MouseEventArgs args)
        {
            if (!Quiz.Rounds.Any()
                || await ConfirmDeleteModal.ShowAsync(false))
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
            if (!Quiz.Rounds.SelectMany(round => round.Questions).Any())
                return;

            ErrorMessages = new List<string>();
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

            if (ErrorMessages.Any())
                ErrorMessagesAlert.Show();
            else if (await ConfirmPublishModal.ShowAsync(false))
                await Quiz.OpenAsync();
        }

        protected override bool ShouldRender()
        {
            var currentStateStamp = Quiz.GetStateStamp();
            if (currentStateStamp == LastStateStamp)
                return false;

            LastStateStamp = currentStateStamp;
            return true;
        }

        public void Dispose()
        {
            Quiz.Updated -= OnUpdatedAsync;
        }
    }
}
