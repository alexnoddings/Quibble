using Quibble.Data.Entites.Games;

namespace Quibble.Api.Games;

[AttributeUsage(AttributeTargets.Method)]
public sealed class RequireGameStateAttribute : Attribute
{
    public GameState State { get; }

    public RequireGameStateAttribute(GameState state)
    {
        State = state;
    }
}
