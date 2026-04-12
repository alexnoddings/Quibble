namespace Quibble.Caching;

public interface ICached
{
    // Used by the cache to determine if the GetOrCreate-ed value was got or created
    public Guid CacheNonce { get; }
}
