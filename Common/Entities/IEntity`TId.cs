namespace Quibble.Common.Entities;

public interface IEntity<out TId>
{
	public TId Id { get; }
}
