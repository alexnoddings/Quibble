using System.ComponentModel;
using Quibble.Data.Entites.Games;

namespace Quibble.Games.Info;

[ImmutableObject(true)]
public sealed class GameInfo
{
    public required Guid Id { get; init; }
    public required GameState State { get; init; }
    public required Guid OwnerId { get; init; }
    public required string Slug { get; init; }
    public required IReadOnlyCollection<GameParticipantInfo> Participants { get; init; }
}
