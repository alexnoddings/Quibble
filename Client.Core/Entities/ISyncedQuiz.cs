using Quibble.Common.Entities;

namespace Quibble.Client.Core.Entities;

public interface ISyncedQuiz : ISyncedEntity, IQuiz, IAsyncDisposable
{
	public ISyncedEntities<ISyncedRound> Rounds { get; }
	public ISyncedEntities<ISyncedParticipant> Participants { get; }

	public bool IsDeleted { get; }

	public Task AddRoundAsync();
	public Task UpdateTitleAsync(string newTitle);
	public Task OpenAsync();
	public Task DeleteAsync();
}
