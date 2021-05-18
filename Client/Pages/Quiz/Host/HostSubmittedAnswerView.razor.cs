using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Quibble.Client.Sync.Entities.HostMode;
using Quibble.Shared.Entities;

namespace Quibble.Client.Pages.Quiz.Host
{
    public partial class HostSubmittedAnswerView : IDisposable
    {
        [Parameter]
        public ISynchronisedHostModeSubmittedAnswer SubmittedAnswer { get; set; } = default!;

        private string LocalPointsString { get; set; } = string.Empty;

        private string CardClass
        {
            get
            {
                if (SubmittedAnswer.Question.State == QuestionState.Locked && SubmittedAnswer.AssignedPoints == -1)
                    return "border-warning";

                return string.Empty;
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            LocalPointsString = SubmittedAnswer.AssignedPoints.ToString("G4");

            SubmittedAnswer.Updated += OnUpdatedAsync;
        }

        private Task OnUpdatedAsync()
        {
            LocalPointsString = SubmittedAnswer.AssignedPoints.ToString("G4");
            return InvokeAsync(StateHasChanged);
        }

        private Task RoundAndUpdatePointsAsync()
        {
            if (!decimal.TryParse(LocalPointsString, out decimal points))
            {
                LocalPointsString = SubmittedAnswer.AssignedPoints.ToString("G4");
                return Task.CompletedTask;
            }

            // Ensure points are a division of 0.25
            points = Math.Round(points * 4, MidpointRounding.ToEven) / 4;
            points = Math.Clamp(points, 0.25m, 10m);

            LocalPointsString = points.ToString("G4");
            return SubmittedAnswer.MarkAsync(points);
        }

        public void Dispose()
        {
            SubmittedAnswer.Updated -= OnUpdatedAsync;
        }
    }
}
