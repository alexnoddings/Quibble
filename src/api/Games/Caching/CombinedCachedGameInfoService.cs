using Quibble.Caching;
using Quibble.Data.Entites.Games;
using Quibble.Games.Info;

namespace Quibble.Games.Caching;

internal sealed class CombinedCachedGameInfoService : IGameInfoService
{
    private readonly GameInfoCacheById _idCache;
    private readonly GameInfoCacheBySlug _slugCache;
    private readonly QuibbleHybridCache _cache;

    public CombinedCachedGameInfoService(
        GameInfoCacheById idCache,
        GameInfoCacheBySlug slugCache,
        QuibbleHybridCache cache
    )
    {
        _idCache = idCache;
        _slugCache = slugCache;
        _cache = cache;
    }

    public ValueTask<GameInfo?> GetGameByIdAsync(Guid id) =>
        _idCache.GetGameByIdAsync(id);

    public ValueTask<GameInfo?> GetGameBySlugAsync(string slug) =>
        _slugCache.GetGameBySlugAsync(slug);

    public ValueTask OnGameStateUpdatedAsync(Guid id) => OnGameInfoMadeStaleAsync(id);
    public ValueTask OnGameParticipantsChangedAsync(Guid id) => OnGameInfoMadeStaleAsync(id);

    public async ValueTask OnGameDeletedAsync(Guid id)
    {
        // Clear all caches tagged with this game
        await _cache.RemoveByTagAsync(
            CacheKey.For<Game>.WithId(id)
        );
    }

    private async ValueTask OnGameInfoMadeStaleAsync(Guid id)
    {
        var gameInfo = await _idCache.GetGameByIdAsync(id);
        if (gameInfo is null)
            return;

        await _idCache.OnGameInfoStaleAsync(id);
        await _slugCache.OnGameInfoStaleAsync(gameInfo.Slug);
        await _cache.RemoveByTagAsync(
            CacheKey.For<Game>.WithId(id)
        );
    }
}
