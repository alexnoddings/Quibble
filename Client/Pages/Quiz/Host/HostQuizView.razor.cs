using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Quibble.Client.Sync.Entities.HostMode;

namespace Quibble.Client.Pages.Quiz.Host
{
    public partial class HostQuizView : IDisposable
    {
        [Parameter]
        public ISynchronisedHostModeQuiz Quiz { get; set; } = default!;

        private SelectionContext Selection { get; set; } = default!;

        private int LastStateStamp { get; set; } = 0;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Selection = new SelectionContext(Quiz);
            Quiz.Updated += OnUpdatedAsync;
            Selection.Updated += OnUpdatedAsync;
            LastStateStamp = Quiz.GetStateStamp();
        }

        private Task OnUpdatedAsync() => InvokeAsync(StateHasChanged);

        public void Dispose()
        {
            Selection.Question.Updated -= OnUpdatedAsync;
            Selection.Round.Updated -= OnUpdatedAsync;
            Quiz.Updated -= OnUpdatedAsync;
            Selection.Updated -= OnUpdatedAsync;
        }
    }
}
