using Quibble.Client.Sync.Core.Entities;

namespace Quibble.Client.Sync.Extensions;

public static class QuestionExtensions
{
    public static ISyncedSubmittedAnswer? TryGetMyAnswer(this ISyncedQuestion syncedQuestion) =>
        syncedQuestion.SubmittedAnswers
            .Where(answer => answer.Submitter.IsCurrentUser)
            .FirstOrDefault();
}
