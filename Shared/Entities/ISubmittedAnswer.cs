namespace Quibble.Shared.Entities
{
    public interface ISubmittedAnswer : IEntity
    {
        public Guid QuestionId { get; }
        public Guid ParticipantId { get; }
        public string Text { get; }
        public decimal AssignedPoints { get; }
    }
}
