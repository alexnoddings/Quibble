using Microsoft.AspNetCore.Components;
using Quibble.Client.Sync.Entities.TakeMode;

namespace Quibble.Client.Pages.Quiz.Take
{
    public sealed partial class TakeParticipantList : IDisposable
    {
        [Parameter]
        public ISyncedTakeModeQuiz Quiz { get; set; } = default!;

        private int LastStateStamp { get; set; } = 0;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Quiz.Updated += OnUpdatedAsync;
            LastStateStamp = GetStateStamp();
        }

        private Task OnUpdatedAsync() => InvokeAsync(StateHasChanged);


        protected override bool ShouldRender()
        {
            var currentStateStamp = GetStateStamp();
            if (currentStateStamp == LastStateStamp)
                return false;

            LastStateStamp = currentStateStamp;
            return true;
        }

        private int GetStateStamp()
        {
            var hashCode = new HashCode();
            foreach (var participant in Quiz.Participants)
                hashCode.Add(participant.UserName);
            return hashCode.ToHashCode();
        }

        public void Dispose()
        {
            Quiz.Updated -= OnUpdatedAsync;
        }
    }
}
