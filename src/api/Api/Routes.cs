namespace Quibble.Api;

public static class Routes
{
    public const string GameIdKey = "GameId";
    public const string GameId = $"{{{GameIdKey}:guid}}";
    public const string GamesBase = "games";

    public const string RoundIdKey = "RoundId";
    public const string RoundId = $"{{{RoundIdKey}:guid}}";
    public const string RoundsBase = "rounds";

    public const string QuestionIdKey = "QuestionId";
    public const string QuestionId = $"{{{QuestionIdKey}:guid}}";
    public const string QuestionsBase = "questions";

    public const string ParticipantIdKey = "ParticipantId";
    public const string ParticipantId = $"{{{ParticipantIdKey}:guid}}";
}
