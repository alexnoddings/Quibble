using Microsoft.AspNetCore.SignalR;
using Quibble.Games.Events;

namespace Quibble.Api.Games;

public sealed class GameEvents
{
    private readonly Guid _gameId;
    private readonly IHubContext<GameEventsHub, IGameEventsClients> _hub;

    public GameEvents(
        Guid gameId,
        IHubContext<GameEventsHub, IGameEventsClients> hub
    )
    {
        _gameId = gameId;
        _hub = hub;
    }

    public GameEventsGroup SendToEveryone()
    {
        var groupName = GameEventGroups.Everyone(_gameId);
        return Group(groupName);
    }

    public GameEventsGroup SendToHost()
    {
        var groupName = GameEventGroups.Host(_gameId);
        return Group(groupName);
    }

    public GameEventsGroup SendToParticipants()
    {
        var groupName = GameEventGroups.Participants(_gameId);
        return Group(groupName);
    }

    public GameEventsGroup SendToParticipant(Guid participantId)
    {
        var groupName = GameEventGroups.Participant(_gameId, participantId);
        return Group(groupName);
    }

    private GameEventsGroup Group(string groupName)
    {
        var group = _hub.Clients.Group(groupName);
        return new GameEventsGroup(group);
    }

    public static ValueTask<GameEvents?> BindAsync(HttpContext context)
    {
        if (context.QuibbleContext.Game is not { } gameInfo)
            return ValueTask.FromResult<GameEvents?>(null);

        var hub = context.RequestServices.GetRequiredService<IHubContext<GameEventsHub, IGameEventsClients>>();
        var gameEvents = new GameEvents(gameInfo.Id, hub);
        return ValueTask.FromResult<GameEvents?>(gameEvents);
    }
}
