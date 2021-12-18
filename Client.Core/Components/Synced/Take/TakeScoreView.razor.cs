using Quibble.Client.Core.Entities;
using Quibble.Client.Core.Extensions;
using Quibble.Common.Entities;

namespace Quibble.Client.Core.Components.Synced.Take;

public abstract partial class TakeScoreView
{
	protected abstract IEnumerable<ISyncedQuestion> Questions { get; }

	protected bool IsVisible => Questions.Any(q => q.State >= Common.Entities.QuestionState.AnswerRevealed);

	private record Points(decimal Total, decimal Scored, decimal Percent);

	private Points CalculatePoints()
	{
		var totalPoints =
			Questions
				.Where(question => question.State == QuestionState.AnswerRevealed)
				.Sum(question => question.Points);
		var scoredPoints =
			Questions
				.Where(question => question.State == QuestionState.AnswerRevealed)
				.Sum(question => question.TryGetCurrentUsersAnswer()?.AssignedPoints ?? 0);
		var percent =
			totalPoints == 0
			? 0
			: scoredPoints / totalPoints * 100;

		return new Points(totalPoints, scoredPoints, percent);
	}
}
