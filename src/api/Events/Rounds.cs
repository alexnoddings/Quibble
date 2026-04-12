using Quibble.Data.Entites.Rounds;

namespace Quibble.Games.Events;

public abstract record RoundEvent
{
    public required Guid RoundId { get; init; }
}

public record RoundAddedEvent : RoundEvent
{
    public required int Order { get; init; }
    public required RoundState State { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
}

public record RoundRemovedEvent : RoundEvent
{
}

public record RoundOrderChangedEvent : RoundEvent
{
    public required int Order { get; init; }
}

public record RoundStateChangedEvent : RoundEvent
{
    public required RoundState State { get; init; }
}

public record RoundTitleChangedEvent : RoundEvent
{
    public required string Title { get; init; }
}

public record RoundDescriptionChangedEvent : RoundEvent
{
    public required string Description { get; init; }
}
