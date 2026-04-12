using System.Reflection;

namespace Quibble.Api.Answers;

public static class RequireAnswerExistsEndpointFilter
{
    public static EndpointFilterDelegate Factory(
        EndpointFilterFactoryContext factoryContext,
        EndpointFilterDelegate next
    )
    {
        if (factoryContext.MethodInfo.GetCustomAttribute<RequireAnswerAttribute>() is null)
            return next;

        return context => EnsureAnswerExistsAsync(context, next);
    }

    private static ValueTask<object?> EnsureAnswerExistsAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next
    )
    {
        ContextInjector.EnsureContextInjected(context.HttpContext);

        if (context.HttpContext.QuibbleContext.Answer is null)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            return ValueTask.FromResult<object?>(null);
        }

        return next(context);
    }
}
