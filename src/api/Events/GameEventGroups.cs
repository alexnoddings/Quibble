namespace Quibble.Games.Events;

public static class GameEventGroups
{
    private static string Base(Guid gameId) => $"game/{gameId:N}";

    public static string Everyone(Guid gameId) => Base(gameId) + "/everyone";
    public static string Host(Guid gameId) => Base(gameId) + "/host";
    public static string Participants(Guid gameId) => Base(gameId) + "/participants";
    public static string Participant(Guid gameId, Guid participantId) => Participants(gameId) + $"/{participantId:N}";
}
