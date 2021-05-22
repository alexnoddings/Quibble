using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Quibble.Client.Components.Modals;
using Quibble.Client.Sync.Entities.EditMode;

namespace Quibble.Client.Pages.Quiz.Edit
{
    public sealed partial class EditQuestionView : IDisposable
    {
        [Parameter]
        public ISynchronisedEditModeQuestion Question { get; set; } = default!;

        private OptionsModal<bool> ConfirmDeleteModal { get; set; } = default!;

        [Parameter]
        public int Index { get; set; }

        private int PreviousIndex { get; set; }

        private int LastStateStamp { get; set; } = 0;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Question.Updated += OnUpdatedAsync;
            LastStateStamp = Question.GetStateStamp();
            PreviousIndex = Index;
        }

        private Task OnUpdatedAsync() =>
            InvokeAsync(StateHasChanged);

        private async Task OnDeleteClickedAsync(MouseEventArgs args)
        {
            if (args.ShiftKey
                || (string.IsNullOrWhiteSpace(Question.Text) && string.IsNullOrWhiteSpace(Question.Answer))
                || await ConfirmDeleteModal.ShowAsync(false))
            {
                await Question.DeleteAsync();
            }
        }

        protected override bool ShouldRender()
        {
            if (Index != PreviousIndex)
            {
                PreviousIndex = Index;
                return true;
            }

            var currentStateStamp = Question.GetStateStamp();
            if (currentStateStamp == LastStateStamp)
                return false;

            LastStateStamp = currentStateStamp;
            return true;
        }

        public void Dispose()
        {
            Question.Updated -= OnUpdatedAsync;
        }
    }
}
