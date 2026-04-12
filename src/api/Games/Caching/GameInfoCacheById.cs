using Microsoft.EntityFrameworkCore;
using Quibble.Caching;
using Quibble.Data;
using Quibble.Data.Entites.Games;

namespace Quibble.Games.Caching;

internal sealed class GameInfoCacheById : GameInfoCache<Guid>
{
    public GameInfoCacheById(QuibbleHybridCache cache, IDbContextFactory<QuibbleDbContext> dbContextFactory) : base(cache, dbContextFactory)
    {
    }

    protected override string GetCacheKey(Guid id) => CacheKey.For<Info.GameInfo>.WithId(id);
    protected override IQueryable<Game> FilterGamesBy(Guid id, IQueryable<Game> games) => games.Where(g => g.Id == id);

    public ValueTask<Info.GameInfo?> GetGameByIdAsync(Guid id) => GetGameFromCacheAsync(id);
}
