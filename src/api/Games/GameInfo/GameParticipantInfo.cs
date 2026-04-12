using System.ComponentModel;

namespace Quibble.Games.Info;

[ImmutableObject(true)]
public sealed class GameParticipantInfo
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
}
