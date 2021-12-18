using Quibble.Client.Core.Entities;

namespace Quibble.Client.Core.Extensions;

public static class QuestionExtensions
{
	public static ISyncedSubmittedAnswer? TryGetCurrentUsersAnswer(this ISyncedQuestion syncedQuestion) =>
		syncedQuestion.SubmittedAnswers
			.FirstOrDefault(answer => answer.Submitter.IsCurrentUser);
}
