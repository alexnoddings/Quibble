using Quibble.Data.Entites.Questions;

namespace Quibble.Games.Events;

public abstract record QuestionEvent
{
    public required Guid QuestionId { get; init; }
}

public record QuestionAddedEvent : QuestionEvent
{
    public required Guid RoundId { get; init; }
    public required int Order { get; init; }
    public required QuestionState State { get; init; }
    public required decimal Points { get; init; }
    public required string BodyText { get; init; }
    public required string AnswerText { get; init; }
}

public record QuestionRevealedEvent : QuestionEvent
{
    public required string BodyText { get; init; }
    public required string AnswerText { get; init; }
}

public record QuestionRemovedEvent : QuestionEvent
{
}

public record QuestionOrderChangedEvent : QuestionEvent
{
    public required int Order { get; init; }
}

public record QuestionStateChangedEvent : QuestionEvent
{
    public required QuestionState State { get; init; }
}

public record QuestionPointsChangedEvent : QuestionEvent
{
    public required decimal Points { get; init; }
}

public record QuestionBodyTextChangedEvent : QuestionEvent
{
    public required string BodyText { get; init; }
}

public record QuestionAnswerTextChangedEvent : QuestionEvent
{
    public required string AnswerText { get; init; }
}
