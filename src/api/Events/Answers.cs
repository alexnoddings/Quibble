namespace Quibble.Games.Events;

public abstract record AnswerEvent
{
    public required Guid QuestionId { get; init; }
    public required Guid ParticipantId { get; init; }
}

public record AnswerTextChangedEvent : AnswerEvent
{
    public required string Answer { get; init; }
}

public record AnswerTextPreviewedEvent : AnswerEvent
{
    public required string Answer { get; init; }
}

public record AnswerPointsChangedEvent : AnswerEvent
{
    public required decimal Points { get; init; }
}
