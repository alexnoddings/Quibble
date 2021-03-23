namespace Quibble.Shared.Models
{
    public interface IEntity<out TId>
    {
        public TId Id { get; }
    }
}
