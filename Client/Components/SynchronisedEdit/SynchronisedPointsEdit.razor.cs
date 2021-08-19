using Microsoft.AspNetCore.Components;

namespace Quibble.Client.Components.SynchronisedEdit
{
    public partial class SynchronisedPointsEdit
    {
        [Parameter]
        public decimal Points { get; set; }

        private string LocalPointsString { get; set; } = string.Empty;

        [Parameter]
        public Func<decimal, Task> SaveFunction { get; set; } = default!;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            LocalPointsString = Points.ToString("G4");
        }

        private Task RoundAndUpdatePointsAsync()
        {
            if (!decimal.TryParse(LocalPointsString, out decimal points))
            {
                LocalPointsString = Points.ToString("G4");
                return Task.CompletedTask;
            }

            // Ensure points are a division of 0.25
            points = Math.Round(points * 4, MidpointRounding.ToEven) / 4;
            points = Math.Clamp(points, 0.25m, 10m);

            LocalPointsString = points.ToString("G4");
            return SaveFunction(points);
        }
    }
}
