using Microsoft.AspNetCore.Components;

namespace Quibble.Client.Core.Components.Synced;

public partial class SynchronisedPointsEdit
{
	[Parameter]
	public decimal Points { get; set; }

	private decimal LocalPoints { get; set; }

	[Parameter]
	public Func<decimal, Task> SaveFunction { get; set; } = default!;

	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		LocalPoints = Points;
	}

	private Task UpdatePointsAsync(decimal newPoints)
	{
		LocalPoints = RoundPoints(newPoints);

		if (LocalPoints == Points)
			return Task.CompletedTask;

		return SaveFunction(LocalPoints);
	}

	private static decimal RoundPoints(decimal points)
	{
		// Ensure points are a division of 0.25
		points = Math.Round(points * 4, MidpointRounding.ToEven) / 4;
		return Math.Clamp(points, 0.25m, 10m);
	}
}
