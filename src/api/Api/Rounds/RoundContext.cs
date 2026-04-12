using Quibble.Rounds.Info;

namespace Quibble.Api.Rounds;

public sealed class RoundContext
{
    public RoundInfo Round { get; }

    public RoundContext(RoundInfo round)
    {
        Round = round;
    }

    public static ValueTask<RoundContext?> BindAsync(HttpContext context)
    {
        var roundContext = context.QuibbleContext.Round switch
        {
            { } roundInfo => new RoundContext(roundInfo),
            _ => null
        };
        return ValueTask.FromResult(roundContext);
    }
}
