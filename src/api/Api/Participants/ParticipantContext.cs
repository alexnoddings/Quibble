using Quibble.Games.Info;

namespace Quibble.Api.Users;

public sealed class ParticipantContext
{
    public GameParticipantInfo Participant { get; }

    public ParticipantContext(GameParticipantInfo participant)
    {
        Participant = participant;
    }

    public static ValueTask<ParticipantContext?> BindAsync(HttpContext context)
    {
        if (context.QuibbleContext.Participant is { } participantInfo)
        {
            var participantContext = new ParticipantContext(participantInfo);
            return ValueTask.FromResult<ParticipantContext?>(participantContext);
        }

        return ValueTask.FromResult<ParticipantContext?>(null);
    }
}
