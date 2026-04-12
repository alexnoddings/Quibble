namespace Quibble.Rounds.Info;

internal sealed class CachedRoundInfoService : IRoundInfoService
{
    private readonly RoundInfoCache _cache;

    public CachedRoundInfoService(RoundInfoCache cache)
    {
        _cache = cache;
    }

    public ValueTask<RoundInfo?> GetRoundByIdAsync(Guid id) => _cache.GetRoundByIdAsync(id);
    public ValueTask OnRoundStateUpdatedAsync(Guid id) => _cache.OnRoundInfoStaleAsync(id);
    public ValueTask OnRoundDeletedAsync(Guid id) => _cache.OnRoundDeletedAsync(id);
}
