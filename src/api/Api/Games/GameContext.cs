using Quibble.Games.Info;

namespace Quibble.Api.Games;

public sealed class GameContext
{
    public GameInfo Game { get; }

    public GameContext(GameInfo game)
    {
        Game = game;
    }

    public static ValueTask<GameContext?> BindAsync(HttpContext context)
    {
        var gameContext = context.QuibbleContext.Game switch
        {
            { } gameInfo => new GameContext(gameInfo),
            _ => null
        };
        return ValueTask.FromResult(gameContext);
    }
}
