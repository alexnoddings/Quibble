using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Quibble.Client.Sync.Entities.TakeMode;

namespace Quibble.Client.Pages.Quiz.Take
{
    public sealed partial class TakeQuestionView : IDisposable
    {
        [Parameter]
        public ISynchronisedTakeModeQuestion Question { get; set; } = default!;

        [Parameter]
        public int Index { get; set; }

        private int PreviousIndex { get; set; }

        private int LastStateStamp { get; set; } = 0;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Question.Updated += OnUpdatedAsync;
            if (Question.SubmittedAnswer is not null)
                Question.SubmittedAnswer.Updated += OnUpdatedAsync;
            LastStateStamp = Question.GetStateStamp();
            PreviousIndex = Index;
        }

        private Task OnUpdatedAsync() =>
            InvokeAsync(StateHasChanged);

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
            if (Question.SubmittedAnswer is not null)
                Question.SubmittedAnswer.Updated -= OnUpdatedAsync;
        }
    }
}
