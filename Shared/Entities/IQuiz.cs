namespace Quibble.Shared.Entities;

public interface IQuiz : IEntity
{
    public Guid OwnerId { get; }
    public string Title { get; }
    public QuizState State { get; }
    public DateTime CreatedAt { get; }
    public DateTime? OpenedAt { get; }
}
