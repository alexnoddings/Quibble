using Microsoft.AspNetCore.Components;
using Quibble.Client.Core.Entities;

namespace Quibble.Client.Core.Components.Synced.Take;

public sealed partial class TakeRoundView : IDisposable
{
	[Parameter]
	public ISyncedRound Round { get; set; } = default!;

	protected override void OnInitialized()
	{
		base.OnInitialized();

		Round.Updated += OnUpdatedAsync;
		Round.Questions.Added += OnQuestionAddedAsync;
		foreach (var question in Round.Questions)
		{
			question.Updated += OnUpdatedAsync;
			foreach (var submittedAnswers in question.SubmittedAnswers)
				submittedAnswers.Updated += OnUpdatedAsync;
		}
	}

	private Task OnQuestionAddedAsync(ISyncedQuestion question)
	{
		question.Updated += OnUpdatedAsync;
		foreach (var submittedAnswers in question.SubmittedAnswers)
			submittedAnswers.Updated += OnUpdatedAsync;

		return OnUpdatedAsync();
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
			foreach (var submittedAnswers in question.SubmittedAnswers)
				submittedAnswers.Updated -= OnUpdatedAsync;
		}
	}
}
