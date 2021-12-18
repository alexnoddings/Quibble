using Microsoft.AspNetCore.Components;
using Quibble.Client.Core.Entities;
using Quibble.Client.Core.Extensions;

namespace Quibble.Client.Core.Components.Synced.Take;

public sealed partial class TakeQuizScoreView : TakeScoreView
{
	[Parameter]
	public ISyncedQuiz Quiz { get; set; } = default!;

	protected override IEnumerable<ISyncedQuestion> Questions => Quiz.Rounds.SelectMany(r => r.Questions);

	protected override void OnInitialized()
	{
		base.OnInitialized();

		Quiz.Updated += OnUpdatedAsync;
		Quiz.Rounds.Added += OnRoundAddedAsync;
		foreach (var round in Quiz.Rounds)
		{
			round.Updated += OnUpdatedAsync;
			round.Questions.Added += OnQuestionAddedAsync;
			foreach (var question in round.Questions)
			{
				question.Updated += OnUpdatedAsync;
				var answer = question.TryGetCurrentUsersAnswer();
				if (answer is not null)
					answer.Updated += OnUpdatedAsync;
			}
		}
	}

	private Task OnRoundAddedAsync(ISyncedRound round)
	{
		round.Updated += OnUpdatedAsync;
		round.Questions.Added += OnQuestionAddedAsync;
		return Task.CompletedTask;
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
		StateStamp.ForProperties(Quiz);

	public void Dispose()
	{
		Quiz.Updated -= OnUpdatedAsync;
		Quiz.Rounds.Added -= OnRoundAddedAsync;
		foreach (var round in Quiz.Rounds)
		{
			round.Updated -= OnUpdatedAsync;
			round.Questions.Added -= OnQuestionAddedAsync;
			foreach (var question in round.Questions)
			{
				question.Updated -= OnUpdatedAsync;
				var answer = question.TryGetCurrentUsersAnswer();
				if (answer is not null)
					answer.Updated += OnUpdatedAsync;
			}
		}
	}
}
