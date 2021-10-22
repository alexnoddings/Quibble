using Quibble.Client.Sync.Core.Entities;
using Quibble.Client.Sync.Extensions;
using System.Collections;

namespace Quibble.Client.Sync.Entities;

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
        return Added.InvokeAsync(entity);
    }

    public TEntity this[int i] => Entities[i];

    internal TEntity? TryGet(Guid id) =>
        Entities.FirstOrDefault(entity => entity.Id == id);

    internal Task RemoveAsync(TEntity entity)
    {
        Entities.Remove(entity);
        return Removed.InvokeAsync(entity);
    }

    public IEnumerator<TEntity> GetEnumerator() => Entities.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
