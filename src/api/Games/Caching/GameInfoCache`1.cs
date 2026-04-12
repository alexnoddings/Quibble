using Microsoft.EntityFrameworkCore;
using Quibble.Caching;
using Quibble.Data;
using Quibble.Data.Entites.Games;
using Quibble.Games.Info;

namespace Quibble.Games.Caching;

internal abstract class GameInfoCache<TKey>
{
    private readonly QuibbleHybridCache _cache;
    private readonly IDbContextFactory<QuibbleDbContext> _dbContextFactory;

    protected GameInfoCache(QuibbleHybridCache cache, IDbContextFactory<QuibbleDbContext> dbContextFactory)
    {
        _cache = cache;
        _dbContextFactory = dbContextFactory;
    }

    public ValueTask OnGameInfoStaleAsync(TKey key)
    {
        var cacheKey = GetCacheKey(key);
        return _cache.RemoveAsync(cacheKey);
    }

    protected abstract string GetCacheKey(TKey key);

    protected async ValueTask<GameInfo?> GetGameFromCacheAsync(TKey key)
    {
        var cacheKey = GetCacheKey(key);
        var gameInfo = await _cache.GetOrCreateAsync(
            cacheKey,
            key,
            LoadGameRawAsync,
            static gameInfo => [
                CacheKey.For<Game>.WithId(gameInfo.Id)
            ],
            GameInfoCache._cacheOptions
        );

        return gameInfo;
    }

    protected abstract IQueryable<Game> FilterGamesBy(TKey key, IQueryable<Game> games);

    private async ValueTask<Cached<GameInfo>?> LoadGameRawAsync(TKey key, Guid nonce, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var games = dbContext.Games.AsNoTracking();

        var gameInfo = await
            FilterGamesBy(key, games)
                .Select(g => new GameInfo
                {
                    Id = g.Id,
                    Slug = g.Slug,
                    OwnerId = g.OwnerId,
                    State = g.State,
                    Participants = g.Participants
                        .Select(p => new GameParticipantInfo { Id = p.Id, UserId = p.UserId, })
                        .ToList()
                        .AsReadOnly()
                })
                .FirstOrDefaultAsync(cancellationToken);

        if (gameInfo is null)
            return null;

        return new Cached<GameInfo> { CacheNonce = nonce, Value = gameInfo };
    }
}
