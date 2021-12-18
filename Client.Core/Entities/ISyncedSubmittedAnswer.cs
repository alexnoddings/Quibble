using Quibble.Common.Entities;

namespace Quibble.Client.Core.Entities;

public interface ISyncedSubmittedAnswer : ISyncedEntity, ISubmittedAnswer
{
	public ISyncedQuestion Question { get; }
	public ISyncedParticipant Submitter { get; }

	public Task PreviewUpdateTextAsync(string previewText);
	public Task UpdateTextAsync(string newText);
	public Task MarkAsync(decimal points);
}
