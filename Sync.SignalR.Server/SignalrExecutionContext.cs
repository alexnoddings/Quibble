using Quibble.Server.Core.Sync;

namespace Quibble.Sync.SignalR.Server;

internal class SignalrExecutionContext : IExecutionContext
{
	public Guid UserId { get; }
	public Guid QuizId { get; }
	public ISyncedUsers Users { get; }

	public SignalrExecutionContext(Guid currentUserId, Guid currentQuizId, ISyncedUsers users)
	{
		UserId = currentUserId;
		QuizId = currentQuizId;
		Users = users;
	}
}
