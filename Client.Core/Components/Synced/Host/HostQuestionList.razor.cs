using Microsoft.AspNetCore.Components;
using Quibble.Client.Core;
using Quibble.Client.Core.Entities;

namespace Quibble.Client.Core.Components.Synced.Host;

public sealed partial class HostQuestionList : IDisposable
{
	[Parameter]
	public ISyncedQuiz Quiz { get; set; } = default!;

	[CascadingParameter]
	private SelectionContext Selection { get; init; } = default!;

	private int HighlightedRoundNumber { get; set; }

	protected override void OnInitialized()
	{
		base.OnInitialized();

		Selection.OnUpdated += OnSelectionUpdatedAsync;
		foreach (var round in Quiz.Rounds)
		{
			round.Updated += OnUpdatedAsync;
			foreach (var question in round.Questions)
				question.Updated += OnUpdatedAsync;
		}
	}

	private Task OnSelectionUpdatedAsync(SelectionChangedEventArgs _)
	{
		HighlightedRoundNumber = Selection.RoundNumber;
		return OnUpdatedAsync();
	}

	protected override int CalculateStateStamp() =>
		StateStamp.ForProperties(Quiz, HighlightedRoundNumber, Selection.RoundNumber, Selection.QuestionNumber);

	public void Dispose()
	{
		Selection.OnUpdated -= OnSelectionUpdatedAsync;
		foreach (var round in Quiz.Rounds)
		{
			round.Updated -= OnUpdatedAsync;
			foreach (var question in round.Questions)
				question.Updated -= OnUpdatedAsync;
		}
	}
}
