using Quibble.Common.Entities;

namespace Quibble.Client.Core.Entities;

public interface ISyncedEntity : IEntity
{
	public event Func<Task>? Updated;

	public int GetStateStamp();
}
