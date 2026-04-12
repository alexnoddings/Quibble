using System.Reflection;
using Quibble.Data.Entites.Games;

namespace Quibble.Api.Games;

public static class RequireGameStateEndpointFilter
{
    public static EndpointFilterDelegate Factory(
        EndpointFilterFactoryContext factoryContext,
        EndpointFilterDelegate next
    )
    {
        var gameStateAttribute = factoryContext.MethodInfo.GetCustomAttribute<RequireGameStateAttribute>();
        if (gameStateAttribute is null)
            return next;

        var gameState = gameStateAttribute.State;
        return context => EnsureGameStateAsync(context, gameState, next);
    }

    private static ValueTask<object?> EnsureGameStateAsync(
        EndpointFilterInvocationContext context,
        GameState gameState,
        EndpointFilterDelegate next
    )
    {
        ContextInjector.EnsureContextInjected(context.HttpContext);

        if (context.HttpContext.QuibbleContext.Game is not { } gameInfo)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            return ValueTask.FromResult<object?>(null);
        }

        if (gameInfo.State != gameState)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return ValueTask.FromResult<object?>(null);
        }

        return next(context);
    }
}
