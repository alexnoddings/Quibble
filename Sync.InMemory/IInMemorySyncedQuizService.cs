using Quibble.Client.Core.Contexts;
using Quibble.Client.Core.Factories;

namespace Quibble.Sync.InMemory;

public interface IInMemorySyncedQuizService : ISyncedQuizService
{
	public IDisposable UseUser(Guid userId);
}
