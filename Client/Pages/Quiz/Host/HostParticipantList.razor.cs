using Microsoft.AspNetCore.Components;
using Quibble.Client.Sync.Entities.HostMode;

namespace Quibble.Client.Pages.Quiz.Host
{
    public sealed partial class HostParticipantList : IDisposable
    {
        [Parameter]
        public ISyncedHostModeQuiz Quiz { get; set; } = default!;

        private int LastStateStamp { get; set; } = 0;

        private List<ISyncedHostModeParticipant> KnownParticipants { get; } = new();

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Quiz.Updated += OnUpdatedAsync;
            RegisterUpdateEvents();

            LastStateStamp = GetStateStamp();
        }

        private Task OnUpdatedAsync()
        {
            RegisterUpdateEvents();
            return InvokeAsync(StateHasChanged);
        }

        private void RegisterUpdateEvents()
        {
            foreach (var participant in Quiz.Participants)
            {
                if (KnownParticipants.Contains(participant))
                    continue;

                foreach (var answer in participant.Answers)
                    answer.Updated += OnUpdatedAsync;

                KnownParticipants.Add(participant);
            }
        }

        protected override bool ShouldRender()
        {
            var currentStateStamp = GetStateStamp();
            if (currentStateStamp == LastStateStamp)
                return false;

            LastStateStamp = currentStateStamp;
            return true;
        }

        private IEnumerable<(decimal, List<ISyncedHostModeParticipant>)> GetParticipantScores() =>
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

            foreach (var participant in KnownParticipants)
                foreach (var answer in participant.Answers)
                    answer.Updated -= OnUpdatedAsync;

            KnownParticipants.Clear();
        }
    }
}
