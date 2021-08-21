using Microsoft.AspNetCore.Components;
using Quibble.Client.Sync.Entities.TakeMode;

namespace Quibble.Client.Pages.Quiz.Take
{
    public sealed partial class TakeRoundView : IDisposable
    {
        [Parameter]
        public ISyncedTakeModeRound Round { get; set; } = default!;

        private int LastStateStamp { get; set; } = 0;

        private List<ISyncedTakeModeQuestion> KnownQuestions { get; } = new();

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Round.Updated += OnUpdatedAsync;
            RegisterUpdateEvents();

            LastStateStamp = Round.GetStateStamp();
        }

        private Task OnUpdatedAsync()
        {
            RegisterUpdateEvents();
            return InvokeAsync(StateHasChanged);
        }

        private void RegisterUpdateEvents()
        {
            foreach (var question in Round.Questions)
            {
                if (KnownQuestions.Contains(question))
                    continue;
                
                question.Updated += OnUpdatedAsync;
                if (question.SubmittedAnswer is not null)
                    question.SubmittedAnswer.Updated += OnUpdatedAsync;

                KnownQuestions.Add(question);
            }
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
            foreach (var question in Round.Questions)
            {
                question.Updated -= OnUpdatedAsync;
                if (question.SubmittedAnswer is not null)
                    question.SubmittedAnswer.Updated -= OnUpdatedAsync;
            }
        }
    }
}
