using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Quibble.Core.Entities;
using Quibble.UI.Core.Entities;

namespace Quibble.UI.Features.Take
{
    public sealed partial class TakeRound : IDisposable
    {
        [CascadingParameter]
        public SyncedRound Round { get; set; } = default!;

        [CascadingParameter]
        public SyncedParticipant Participant { get; set; } = default!;

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

        private int CalculateScore() =>
            Round
            .Questions
            .SelectMany(q => q.Answers)
            .Where(a => a.ParticipantId == Participant.Id)
            .Count(a => a.Mark == AnswerMark.Right);
    }
}
