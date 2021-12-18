using Quibble.Server.Core.Sync;

namespace Quibble.Sync.InMemory.Server;

internal class SyncExecutionContext : IExecutionContext
{
	public Guid QuizId { get; }
	public Guid UserId { get; }
	public ISyncedUsers Users { get; }

	public SyncExecutionContext(Guid quizId, Guid userId, ISyncedUsers users)
	{
		QuizId = quizId;
		UserId = userId;
		Users = users ?? throw new ArgumentNullException(nameof(users));
	}
}
