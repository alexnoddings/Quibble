using Microsoft.AspNetCore.Components;
using Quibble.Client.Core.Entities;
using Quibble.Client.Core.Extensions;

namespace Quibble.Client.Core.Components.Synced.Take;

public sealed partial class TakeRoundScoreView : TakeScoreView
{
	[Parameter]
	public ISyncedRound Round { get; set; } = default!;

	protected override IEnumerable<ISyncedQuestion> Questions => Round.Questions;

	protected override void OnInitialized()
	{
		base.OnInitialized();

		Round.Updated += OnUpdatedAsync;
		Round.Questions.Added += OnQuestionAddedAsync;
		foreach (var question in Round.Questions)
		{
			question.Updated += OnUpdatedAsync;
			var answer = question.TryGetCurrentUsersAnswer();
			if (answer is not null)
				answer.Updated += OnUpdatedAsync;
		}
	}

	private Task OnQuestionAddedAsync(ISyncedQuestion question)
	{
		question.Updated += OnUpdatedAsync;
		var answer = question.TryGetCurrentUsersAnswer();
		if (answer is not null)
			answer.Updated += OnUpdatedAsync;
		return Task.CompletedTask;
	}

	protected override int CalculateStateStamp() =>
		StateStamp.ForProperties(Round);

	public void Dispose()
	{
		Round.Updated -= OnUpdatedAsync;
		Round.Questions.Added -= OnQuestionAddedAsync;
		foreach (var question in Round.Questions)
		{
			question.Updated -= OnUpdatedAsync;
			var answer = question.TryGetCurrentUsersAnswer();
			if (answer is not null)
				answer.Updated += OnUpdatedAsync;
		}
	}
}
