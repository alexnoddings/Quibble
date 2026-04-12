using System.Reflection;

namespace Quibble.Api.Rounds;

public static class RequireRoundExistsEndpointFilter
{
    public static EndpointFilterDelegate Factory(
        EndpointFilterFactoryContext factoryContext,
        EndpointFilterDelegate next
    )
    {
        if (factoryContext.MethodInfo.GetCustomAttribute<RequireRoundAttribute>() is null)
            return next;

        return context => EnsureRoundExistsAsync(context, next);
    }

    private static ValueTask<object?> EnsureRoundExistsAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next
    )
    {
        ContextInjector.EnsureContextInjected(context.HttpContext);

        if (context.HttpContext.QuibbleContext.Round is null)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            return ValueTask.FromResult<object?>(null);
        }

        return next(context);
    }
}
