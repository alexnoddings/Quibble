using System.Reflection;
using Quibble.Users;

namespace Quibble.Api.Users;

public static class RequireUserOwnsGameEndpointFilter
{
    public static EndpointFilterDelegate Factory(
        EndpointFilterFactoryContext factoryContext,
        EndpointFilterDelegate next
    )
    {
        if (factoryContext.MethodInfo.GetCustomAttribute<RequireUserOwnsGameAttribute>() is null)
            return next;

        return context => EnsureUserOwnsGameAsync(context, next);
    }

    private static async ValueTask<object?> EnsureUserOwnsGameAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next
    )
    {
        ContextInjector.EnsureContextInjected(context.HttpContext);

        if (context.HttpContext.QuibbleContext.Game is not { } gameInfo)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            return null;
        }

        var userService = context.HttpContext.RequestServices.GetRequiredService<ICurrentUserService>();
        var userInfo = await userService.GetCurrentUserInfoAsync();
        if (userInfo.Id != gameInfo.OwnerId)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            return null;
        }

        return await next(context);
    }
}
