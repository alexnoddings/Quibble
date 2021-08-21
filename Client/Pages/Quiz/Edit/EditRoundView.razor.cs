using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Quibble.Client.Components.Modals;
using Quibble.Client.Sync.Entities.EditMode;

namespace Quibble.Client.Pages.Quiz.Edit
{
    public sealed partial class EditRoundView : IDisposable
    {
        [Parameter]
        public ISyncedEditModeRound Round { get; set; } = default!;

        private OptionsModal<bool> ConfirmDeleteModal { get; set; } = default!;

        private int LastStateStamp { get; set; } = 0;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Round.Updated += OnUpdatedAsync;
            LastStateStamp = Round.GetStateStamp();
        }

        private Task OnUpdatedAsync() =>
            InvokeAsync(StateHasChanged);

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

        protected override bool ShouldRender()
        {
            var currentStateStamp = Round.GetStateStamp();
            if (currentStateStamp == LastStateStamp)
                return false;

            LastStateStamp = currentStateStamp;
            return true;
        }

        public void Dispose()
        {
            Round.Updated -= OnUpdatedAsync;
        }
    }
}
