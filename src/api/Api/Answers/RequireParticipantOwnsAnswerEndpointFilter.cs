using System.Reflection;

namespace Quibble.Api.Answers;

public static class RequireParticipantOwnsAnswerEndpointFilter
{
    public static EndpointFilterDelegate Factory(
        EndpointFilterFactoryContext factoryContext,
        EndpointFilterDelegate next
    )
    {
        if (factoryContext.MethodInfo.GetCustomAttribute<RequireParticipantOwnsAnswerAttribute>() is null)
            return next;

        return context => EnsureParticipantOwnsAnswerAsync(context, next);
    }

    private static ValueTask<object?> EnsureParticipantOwnsAnswerAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next
    )
    {
        ContextInjector.EnsureContextInjected(context.HttpContext);

        // Answer must exist
        if (context.HttpContext.QuibbleContext.Answer is not { } answerInfo)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            return ValueTask.FromResult<object?>(null);
        }

        // Participant must exist
        if (context.HttpContext.QuibbleContext.Participant is not { } participantInfo)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            return ValueTask.FromResult<object?>(null);
        }

        // Answer's participant must be this participant
        if (answerInfo.Participant.Id != participantInfo.Id)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            return ValueTask.FromResult<object?>(null);
        }

        return next(context);
    }
}
