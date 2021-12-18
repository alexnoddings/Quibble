namespace Quibble.Server.Core.Sync;

public interface IExecutionContext
{
	public Guid UserId { get; }
	public Guid QuizId { get; }

	public ISyncedUsers Users { get; }
}
