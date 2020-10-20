using Blazorise;
using Microsoft.AspNetCore.Components;
using Quibble.Core.Entities;
using Quibble.UI.Core.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Quibble.UI.Features.Quiz.Icons
{
    public sealed partial class RoundStateIcon
    {
        [Parameter]
        public SyncedRound Round { get; set; } = default!;

        private IconName StateIconName => GetStateIconName();

        protected override async Task OnInitializedAsync()
        {
            if (Round == null) throw new ArgumentException(nameof(Round));

            await base.OnInitializedAsync();

            Round.Updated += OnRoundUpdatedAsync;
        }

        private Task OnRoundUpdatedAsync() => InvokeAsync(StateHasChanged);

        public void Dispose()
        {
            if (Round != null)
                Round.Updated -= OnRoundUpdatedAsync;
        }

        private IconName GetStateIconName()
        {
            if (Round.State == RoundState.Hidden)
                return IconName.EyeSlash;

            if (Round.Questions.Any(q => q.State == QuestionState.Hidden || q.State == QuestionState.Visible))
                return IconName.LockOpen;

            if (Round.Questions.Any(q => q.State == QuestionState.Locked))
                return IconName.Lock;

            return IconName.Check;
        }
    }
}
