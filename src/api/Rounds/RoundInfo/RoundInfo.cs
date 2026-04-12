using System.ComponentModel;
using Quibble.Data.Entites.Rounds;
using Quibble.Games.Info;

namespace Quibble.Rounds.Info;

[ImmutableObject(true)]
public sealed class RoundInfo
{
    public required GameInfo Game { get; init; }

    public required Guid Id { get; init; }
    public required RoundState State { get; init; }
}
