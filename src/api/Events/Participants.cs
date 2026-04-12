namespace Quibble.Games.Events;

public abstract record ParticipantEvent
{
    public required Guid ParticipantId { get; init; }
}

public record ParticipantAddedAnswer
{
    public Guid QuestionId { get; set; }
    public string Answer { get; set; } = string.Empty;
}

public record ParticipantAddedEvent : ParticipantEvent
{
    public required string Name { get; init; }
    public List<ParticipantAddedAnswer>? Answers { get; init; }
}

