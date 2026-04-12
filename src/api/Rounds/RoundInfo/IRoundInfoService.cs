namespace Quibble.Rounds.Info;

public interface IRoundInfoService
{
    public ValueTask<RoundInfo?> GetRoundByIdAsync(Guid id);

    public ValueTask OnRoundStateUpdatedAsync(Guid id);
    public ValueTask OnRoundDeletedAsync(Guid id);
}
