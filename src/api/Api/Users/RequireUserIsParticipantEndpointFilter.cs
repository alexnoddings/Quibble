using System.Reflection;
using Quibble.Users;

namespace Quibble.Api.Users;

public static class RequireUserIsParticipantEndpointFilter
{
    public static EndpointFilterDelegate Factory(
        EndpointFilterFactoryContext factoryContext,
        EndpointFilterDelegate next
    )
    {
        if (factoryContext.MethodInfo.GetCustomAttribute<RequireUserIsParticipantAttribute>() is null)
            return next;

        return context => EnsureUserIsParticipantAsync(context, next);
    }

    private static async ValueTask<object?> EnsureUserIsParticipantAsync(
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
        var userId = userInfo.Id;
        var participantInfo = gameInfo.Participants.FirstOrDefault(p => p.UserId == userId);
        if (participantInfo is null)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            return null;
        }

        var quibbleContext = context.HttpContext.QuibbleContext;
        quibbleContext.Participant = participantInfo;

        return await next(context);
    }
}
