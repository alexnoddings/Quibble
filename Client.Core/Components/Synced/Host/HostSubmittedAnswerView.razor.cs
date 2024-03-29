﻿using Microsoft.AspNetCore.Components;
using Quibble.Client.Core;
using Quibble.Client.Core.Entities;
using Quibble.Common.Entities;

namespace Quibble.Client.Core.Components.Synced.Host;

public sealed partial class HostSubmittedAnswerView : IDisposable
{
	[Parameter]
	public ISyncedSubmittedAnswer SubmittedAnswer { get; set; } = default!;

	private decimal LocalPoints { get; set; }

	private bool IsExpanded { get; set; }

	private string CardClass =>
		SubmittedAnswer.Question.State == QuestionState.Locked && SubmittedAnswer.AssignedPoints == -1
		? "border-warning"
		: "border-muted";

	protected override void OnInitialized()
	{
		base.OnInitialized();

		LocalPoints = SubmittedAnswer.AssignedPoints;

		SubmittedAnswer.Question.Updated += OnUpdatedAsync;
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
		StateStamp.ForProperties(SubmittedAnswer.Question.State, SubmittedAnswer, LocalPoints, IsExpanded);

	public void Dispose()
	{
		SubmittedAnswer.Question.Updated -= OnUpdatedAsync;
		SubmittedAnswer.Updated -= OnUpdatedAsync;
	}
}
