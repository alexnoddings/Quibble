using Quibble.Client.Core.Entities;
using Quibble.Client.Core.Extensions;
using System.Collections;

namespace Quibble.Client.Core.Entities.Synced;

internal class SyncedEntitiesList<TEntity>
	: ISyncedEntities<TEntity>
	where TEntity : SyncedEntity
{
	private List<TEntity> Entities { get; } = new();

	public event Func<TEntity, Task>? Added;
	public event Func<TEntity, Task>? Removed;

	public int Count => Entities.Count;

	internal Task AddAsync(TEntity entity)
	{
		Entities.Add(entity);
		return Added.TryInvokeAsync(entity);
	}

	internal void AddSilent(TEntity entity) => Entities.Add(entity);

	public TEntity this[int i] => Entities[i];

	internal TEntity? TryGet(Guid id) =>
		Entities.Find(entity => entity.Id == id);

	internal Task RemoveAsync(TEntity entity)
	{
		Entities.Remove(entity);
		return Removed.TryInvokeAsync(entity);
	}

	public IEnumerator<TEntity> GetEnumerator() => Entities.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
