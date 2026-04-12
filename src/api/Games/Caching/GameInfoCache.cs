using Microsoft.Extensions.Caching.Hybrid;

namespace Quibble.Games.Caching;

internal abstract class GameInfoCache
{
    internal static readonly HybridCacheEntryOptions _cacheOptions = new()
    {
        LocalCacheExpiration = TimeSpan.FromSeconds(45),
        Expiration = TimeSpan.FromSeconds(90)
    };
}
