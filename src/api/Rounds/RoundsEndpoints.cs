using Quibble.Rounds.Endpoints;

namespace Quibble.Rounds;

public static class RoundsEndpoints
{
    public static IEndpointRouteBuilder MapRoundsApi(this IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapCreateRound()
            .MapUpdateRoundTitle()
            .MapUpdateRoundDescription()
            .MapUpdateRoundOrder()
            .MapUpdateRoundState()
            .MapDeleteRound();

        return endpoints;
    }
}
