namespace Quibble.Games.Info;

public interface IGameInfoService
{
    public ValueTask<GameInfo?> GetGameByIdAsync(Guid id);
    public ValueTask<GameInfo?> GetGameBySlugAsync(string slug);

    public ValueTask OnGameStateUpdatedAsync(Guid id);
    public ValueTask OnGameParticipantsChangedAsync(Guid id);
    public ValueTask OnGameDeletedAsync(Guid id);
}
