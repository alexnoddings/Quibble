using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;
using Quibble.Games.Info;
using Quibble.Users;

namespace Quibble.Games.Events;

public class GameEventsHub : Hub<IGameEventsClients>
{
    private readonly ILogger _logger;
    private readonly IGameInfoService _gameInfoService;
    private readonly IUserService _userService;

    public GameEventsHub(ILogger<GameEventsHub> logger, IGameInfoService gameInfoService, IUserService userService)
    {
        _logger = logger;
        _gameInfoService = gameInfoService;
        _userService = userService;
    }

    public override async Task OnConnectedAsync()
    {
        if (GetGameId() is not { } gameId)
        {
            _logger.LogDebug("GameId is not valid.");
            Context.Abort();
            return;
        }

        if (Context.User is not { } userClaims)
        {
            _logger.LogDebug("User is not authenticated.");
            Context.Abort();
            return;
        }

        var userInfo = await _userService.TryGetUserInfoAsync(userClaims);
        if (userInfo is null)
        {
            _logger.LogDebug("User is not valid.");
            Context.Abort();
            return;
        }

        var gameInfo = await _gameInfoService.GetGameByIdAsync(gameId);
        if (gameInfo is null)
        {
            _logger.LogDebug("Game not found.");
            Context.Abort();
            return;
        }

        var isHost = userInfo.Id == gameInfo.OwnerId;
        if (isHost)
        {
            _logger.LogDebug("User is a host.");
            await AddCurrentUserToGroupsAsync(
                GameEventGroups.Everyone(gameId),
                GameEventGroups.Host(gameId)
            );
            return;
        }

        var userId = userInfo.Id;
        var participant = gameInfo.Participants.FirstOrDefault(p => p.UserId == userId);
        if (participant is not null)
        {
            _logger.LogDebug("User is a participant.");
            await AddCurrentUserToGroupsAsync(
                GameEventGroups.Everyone(gameId),
                GameEventGroups.Participant(gameId, participant.Id),
                GameEventGroups.Participants(gameId)
            );
            return;
        }

        _logger.LogDebug("User is not a participant.");
        Context.Abort();
    }

    private async Task AddCurrentUserToGroupsAsync(params string[] groupNames)
    {
        foreach (var groupName in groupNames)
            await Groups.AddToGroupAsync(this.Context.ConnectionId, groupName);
    }

    private Guid? GetGameId()
    {
        if (Context.GetHttpContext() is not { } httpContext)
            return null;

        var httpPath = httpContext.Request.Path.Value;

        var match = RegexPathHack.Instance().Match(httpPath!);
        var gameIdStr = match.Groups["GameId"].Value;

        if (!Guid.TryParse(gameIdStr, out var gameId))
            return null;

        // if (!httpContext.Request.RouteValues.TryGetValue(Routes.GameIdKey, out Guid gameId))
        //     return null;

        return gameId;
    }
}

public static partial class RegexPathHack
{
    [GeneratedRegex("/api/games/(?<GameId>[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12})/events",
        RegexOptions.ExplicitCapture)]
    public static partial Regex Instance();
}
