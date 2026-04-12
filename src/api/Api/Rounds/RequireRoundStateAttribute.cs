using Quibble.Data.Entites.Rounds;

namespace Quibble.Api.Rounds;

[AttributeUsage(AttributeTargets.Method)]
public sealed class RequireRoundStateAttribute : Attribute
{
    public RoundState State { get; }

    public RequireRoundStateAttribute(RoundState state)
    {
        State = state;
    }
}
