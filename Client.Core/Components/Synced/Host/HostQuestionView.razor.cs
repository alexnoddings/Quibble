using Microsoft.AspNetCore.Components;
using Quibble.Client.Core.Entities;
using Quibble.Common.Entities;

namespace Quibble.Client.Core.Components.Synced.Host;

public sealed partial class HostQuestionView : IDisposable
{
	[Parameter]
	public ISyncedQuiz Quiz { get; set; } = default!;

	[CascadingParameter]
	private SelectionContext Selection { get; init; } = default!;

	protected override void OnInitialized()
	{
		base.OnInitialized();

		var question = Selection.Question;
		question.Round.Updated += OnUpdatedAsync;
		question.Updated += OnUpdatedAsync;
		foreach (var answer in question.SubmittedAnswers)
			answer.Updated += OnUpdatedAsync;

		Selection.OnUpdated += OnQuestionChangedAsync;
	}

	private Task OnQuestionChangedAsync(SelectionChangedEventArgs args)
	{
		args.PreviousQuestion.Round.Updated -= OnUpdatedAsync;
		args.PreviousQuestion.Updated -= OnUpdatedAsync;
		foreach (var answer in args.PreviousQuestion.SubmittedAnswers)
			answer.Updated -= OnUpdatedAsync;

		args.NewQuestion.Round.Updated += OnUpdatedAsync;
		args.NewQuestion.Updated += OnUpdatedAsync;
		foreach (var answer in args.NewQuestion.SubmittedAnswers)
			answer.Updated += OnUpdatedAsync;

		return OnUpdatedAsync();
	}

	private async Task ShowAllRoundsAsync()
	{
		foreach (var round in Quiz.Rounds)
			if (round.State == RoundState.Hidden)
				await round.OpenAsync();
	}

	private async Task ShowAllQuestionsInRoundAsync()
	{
		foreach (var question in Selection.Round.Questions)
			if (question.State == QuestionState.Hidden)
				await question.UpdateStateAsync(QuestionState.Open);
	}

	private async Task LockAllQuestionsInRoundAsync()
	{
		foreach (var question in Selection.Round.Questions)
			if (question.State == QuestionState.Open)
				await question.UpdateStateAsync(QuestionState.Locked);
	}

	private async Task ShowAllQuestionAnswersInRoundAsync()
	{
		foreach (var question in Selection.Round.Questions)
			if (question.State == QuestionState.Locked && question.SubmittedAnswers.All(answer => answer.AssignedPoints >= 0))
				await question.UpdateStateAsync(QuestionState.AnswerRevealed);
	}

	protected override int CalculateStateStamp() =>
		StateStamp.ForProperties(Selection.RoundNumber, Selection.Round.State, Selection.Question);

	public void Dispose()
	{
		Selection.OnUpdated -= OnQuestionChangedAsync;

		var question = Selection.Question;
		question.Round.Updated -= OnUpdatedAsync;
		question.Updated -= OnUpdatedAsync;
		foreach (var answer in question.SubmittedAnswers)
			answer.Updated += OnUpdatedAsync;
	}
}
