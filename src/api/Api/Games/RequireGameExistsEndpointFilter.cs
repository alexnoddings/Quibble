using System.Reflection;

namespace Quibble.Api.Games;

public static class RequireGameExistsEndpointFilter
{
    public static EndpointFilterDelegate Factory(
        EndpointFilterFactoryContext factoryContext,
        EndpointFilterDelegate next
    )
    {
        if (factoryContext.MethodInfo.GetCustomAttribute<RequireGameAttribute>() is null)
            return next;

        return context => EnsureGameExistsAsync(context, next);
    }

    private static ValueTask<object?> EnsureGameExistsAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next
    )
    {
        ContextInjector.EnsureContextInjected(context.HttpContext);

        if (context.HttpContext.QuibbleContext.Game is null)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            return ValueTask.FromResult<object?>(null);
        }

        return next(context);
    }
}
