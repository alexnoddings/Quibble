using System.Reflection;
using Quibble.Data.Entites.Rounds;

namespace Quibble.Api.Rounds;

public static class RequireRoundStateEndpointFilter
{
    public static EndpointFilterDelegate Factory(
        EndpointFilterFactoryContext factoryContext,
        EndpointFilterDelegate next
    )
    {
        var roundStateAttribute = factoryContext.MethodInfo.GetCustomAttribute<RequireRoundStateAttribute>();
        if (roundStateAttribute is null)
            return next;

        var roundState = roundStateAttribute.State;
        return context => EnsureRoundStateAsync(context, roundState, next);
    }

    private static ValueTask<object?> EnsureRoundStateAsync(
        EndpointFilterInvocationContext context,
        RoundState roundState,
        EndpointFilterDelegate next
    )
    {
        ContextInjector.EnsureContextInjected(context.HttpContext);

        if (context.HttpContext.QuibbleContext.Round is not { } roundInfo)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            return ValueTask.FromResult<object?>(null);
        }

        if (roundInfo.State != roundState)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return ValueTask.FromResult<object?>(null);
        }

        return next(context);
    }
}
