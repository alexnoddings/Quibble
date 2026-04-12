using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Quibble.Caching;
using Quibble.Data;
using Quibble.Data.Entites.Games;
using Quibble.Data.Entites.Rounds;
using Quibble.Games.Info;

namespace Quibble.Rounds.Info;

internal sealed class RoundInfoCache
{
    private static readonly HybridCacheEntryOptions _cacheOptions = new()
    {
        LocalCacheExpiration = TimeSpan.FromSeconds(45),
        Expiration = TimeSpan.FromSeconds(90)
    };

    private readonly QuibbleHybridCache _cache;
    private readonly IDbContextFactory<QuibbleDbContext> _dbContextFactory;

    public RoundInfoCache(QuibbleHybridCache cache, IDbContextFactory<QuibbleDbContext> dbContextFactory)
    {
        _cache = cache;
        _dbContextFactory = dbContextFactory;
    }

    public ValueTask OnRoundInfoStaleAsync(Guid id)
    {
        var cacheKey = CacheKey.For<RoundInfo>.WithId(id);
        return _cache.RemoveAsync(cacheKey);
    }

    public ValueTask OnRoundDeletedAsync(Guid id)
    {
        var cacheTag = CacheKey.For<Round>.WithId(id);
        return _cache.RemoveByTagAsync(cacheTag);
    }

    public async ValueTask<RoundInfo?> GetRoundByIdAsync(Guid id)
    {
        var cacheKey = CacheKey.For<RoundInfo>.WithId(id);
        var roundInfo = await _cache.GetOrCreateAsync(
            cacheKey,
            id,
            LoadRoundRawAsync,
            static roundInfo =>
            [
                CacheKey.For<Game>.WithId(roundInfo.Game.Id),
                CacheKey.For<Round>.WithId(roundInfo.Id)
            ],
            _cacheOptions
        );

        return roundInfo;
    }

    private async ValueTask<Cached<RoundInfo>?> LoadRoundRawAsync(Guid id, Guid nonce, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var roundInfo = await dbContext
                .Rounds
                .AsNoTracking()
                .Where(r => r.Id == id)
                .Select(r => new RoundInfo
                {
                    Id = r.Id,
                    State = r.State,
                    Game = new()
                    {
                        Id = r.GameId,
                        Slug = r.Game.Slug,
                        OwnerId = r.Game.OwnerId,
                        State = r.Game.State,
                        Participants = r.Game.Participants
                            .Select(p => new GameParticipantInfo { Id = p.Id, UserId = p.UserId, })
                            .ToList()
                            .AsReadOnly()
                    }
                })
                .FirstOrDefaultAsync(cancellationToken);

        if (roundInfo is null)
            return null;

        return new Cached<RoundInfo> { CacheNonce = nonce, Value = roundInfo };
    }
}
