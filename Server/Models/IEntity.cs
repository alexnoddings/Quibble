namespace Quibble.Server.Models
{
    /// <summary>
    /// Represents an entity with an identifier.
    /// </summary>
    /// <typeparam name="TId">The type of the identifier.</typeparam>
    public interface IEntity<out TId>
    {
        /// <summary>
        /// The entity's identifier.
        /// </summary>
        TId Id { get; }
    }
}
