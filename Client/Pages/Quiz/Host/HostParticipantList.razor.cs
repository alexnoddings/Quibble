using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Quibble.Client.Sync.Entities.HostMode;

namespace Quibble.Client.Pages.Quiz.Host
{
    public partial class HostParticipantList : IDisposable
    {
        [Parameter]
        public ISynchronisedHostModeQuiz Quiz { get; set; } = default!;

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

        private IEnumerable<(decimal, List<ISynchronisedHostModeParticipant>)> GetParticipantScores() =>
            from participant in Quiz.Participants
            let score = participant
                .Answers
                .Select(answer => answer.AssignedPoints)
                .Where(assignedPoints => assignedPoints >= 0)
                .Sum()
            group participant by score
            into groupedParticipants
            orderby groupedParticipants.Key descending
            select (groupedParticipants.Key, groupedParticipants.ToList());

        private int GetStateStamp()
        {
            var hashCode = new HashCode();
            foreach (var participant in Quiz.Participants)
            foreach (var answer in participant.Answers)
                hashCode.Add(answer.AssignedPoints);

            return hashCode.ToHashCode();
        }

        public void Dispose()
        {
            Quiz.Updated -= OnUpdatedAsync;
        }
    }
}
