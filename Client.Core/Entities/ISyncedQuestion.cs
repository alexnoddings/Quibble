using Quibble.Common.Entities;

namespace Quibble.Client.Core.Entities;

public interface ISyncedQuestion : ISyncedEntity, IQuestion
{
	public ISyncedRound Round { get; }
	public ISyncedEntities<ISyncedSubmittedAnswer> SubmittedAnswers { get; }

	public Task UpdateTextAsync(string newText);
	public Task UpdateAnswerAsync(string newAnswer);
	public Task UpdatePointsAsync(decimal newPoints);
	public Task UpdateStateAsync(QuestionState newState);
	public Task DeleteAsync();
}
