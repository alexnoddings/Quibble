using Quibble.Data.Entites.Games;

namespace Quibble.Games.Events;

public abstract record GameEvent
{
}

public record GameTitleChangedEvent : GameEvent
{
    public required string Title { get; init; }
}

public record GameStateChangedEvent : GameEvent
{
    public required GameState State { get; init; }
}
