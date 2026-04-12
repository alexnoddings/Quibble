using System.Reflection;

namespace Quibble.Api.Questions;

public static class RequireQuestionExistsEndpointFilter
{
    public static EndpointFilterDelegate Factory(
        EndpointFilterFactoryContext factoryContext,
        EndpointFilterDelegate next
    )
    {
        if (factoryContext.MethodInfo.GetCustomAttribute<RequireQuestionAttribute>() is null)
            return next;

        return context => EnsureQuestionExistsAsync(context, next);
    }

    private static ValueTask<object?> EnsureQuestionExistsAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next
    )
    {
        ContextInjector.EnsureContextInjected(context.HttpContext);

        if (context.HttpContext.QuibbleContext.Question is null)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            return ValueTask.FromResult<object?>(null);
        }

        return next(context);
    }
}
