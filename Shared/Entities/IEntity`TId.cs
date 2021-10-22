namespace Quibble.Shared.Entities;

public interface IEntity<out TId>
{
    public TId Id { get; }
}
