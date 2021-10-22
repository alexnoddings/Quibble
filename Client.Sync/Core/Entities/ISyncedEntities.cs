namespace Quibble.Client.Sync.Core.Entities;

public interface ISyncedEntities<out TEntity>
    : IEnumerable<TEntity>
    where TEntity : ISyncedEntity
{
    public event Func<TEntity, Task> Added;
    public event Func<TEntity, Task> Removed;

    public int Count { get; }
    public TEntity this[int i] { get; }
}
