namespace Quibble.Common.Entities;

public interface IRound : IEntity
{
	public Guid QuizId { get; }
	public string Title { get; }
	public RoundState State { get; }
	public int Order { get; }
}
