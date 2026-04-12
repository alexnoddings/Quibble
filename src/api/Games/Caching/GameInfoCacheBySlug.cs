using Microsoft.EntityFrameworkCore;
using Quibble.Caching;
using Quibble.Data;
using Quibble.Data.Entites.Games;

namespace Quibble.Games.Caching;

internal sealed class GameInfoCacheBySlug : GameInfoCache<string>
{
    public GameInfoCacheBySlug(QuibbleHybridCache cache, IDbContextFactory<QuibbleDbContext> dbContextFactory) : base(cache, dbContextFactory)
    {
    }

    protected override string GetCacheKey(string slug) => CacheKey.For<Info.GameInfo>.WithProperty("slug", slug);
    protected override IQueryable<Game> FilterGamesBy(string slug, IQueryable<Game> games) => games.Where(g => g.Slug == slug);

    public ValueTask<Info.GameInfo?> GetGameBySlugAsync(string slug) => GetGameFromCacheAsync(slug);
}
