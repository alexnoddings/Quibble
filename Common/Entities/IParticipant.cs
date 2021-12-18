namespace Quibble.Common.Entities;

public interface IParticipant : IEntity
{
	public Guid QuizId { get; }
}
