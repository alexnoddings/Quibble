using Microsoft.AspNetCore.Components;
using Quibble.Client.Sync;
using Quibble.Client.Sync.Core;
using Quibble.Shared.Entities;

namespace Quibble.Client.Pages.Quiz.Host
{
    public sealed partial class HostSubmittedAnswerView : IDisposable
    {
        [Parameter]
        public ISyncedSubmittedAnswer SubmittedAnswer { get; set; } = default!;

        private decimal LocalPoints { get; set; }

        private bool IsExpanded { get; set; }

        private string CardClass =>
            SubmittedAnswer.Question.State == QuestionState.Locked && SubmittedAnswer.AssignedPoints == -1
            ? "border-warning"
            : string.Empty;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            LocalPoints = SubmittedAnswer.AssignedPoints;

            SubmittedAnswer.Updated += OnUpdatedAsync;
        }

        protected override void OnParametersSet()
        {
            LocalPoints = SubmittedAnswer.AssignedPoints;
        }

        private Task RoundAndUpdatePointsAsync(decimal newValue)
        {
            // Ensure points are a division of 0.25
            newValue = Math.Round(newValue * 4, MidpointRounding.ToEven) / 4;
            LocalPoints = Math.Clamp(newValue, 0, 10m);

            return SubmittedAnswer.MarkAsync(LocalPoints);
        }

        protected override int CalculateStateStamp() =>
            StateStamp.ForProperties(SubmittedAnswer, LocalPoints, IsExpanded);

        public void Dispose()
        {
            SubmittedAnswer.Updated -= OnUpdatedAsync;
        }
    }
}
