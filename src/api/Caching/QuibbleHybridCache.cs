using Microsoft.Extensions.Caching.Hybrid;

namespace Quibble.Caching;

public class QuibbleHybridCache : HybridCache
{
    private readonly HybridCache _cacheImpl;
    private readonly CacheMetrics _metrics;

    public QuibbleHybridCache(HybridCache cacheImpl, CacheMetrics metrics)
    {
        _cacheImpl = cacheImpl;
        _metrics = metrics;
    }

    public delegate ValueTask<Cached<TValue>?> CacheFactory<in TState, TValue>(
        TState state,
        Guid nonce,
        CancellationToken cancellation
    );

    private readonly struct CapturedState<TState, TValue>(
        TState originalState,
        Guid nonce,
        CacheFactory<TState, TValue> originalFactory
    )
    {
        public readonly TState OriginalState = originalState;
        public readonly Guid Nonce = nonce;
        public readonly CacheFactory<TState, TValue> OriginalFactory = originalFactory;
    }

    // HybridCache's API requires us to declare cache keys upfront.
    // That doesn't work for children of Games, since we may need to invalidate
    // the child cache when the parent is invalidated.
    // But we can't know the child's GameId until *after* we load the child's data.
    // FusionCache exists as a viable solution to the problem, but as a quick fix we:
    // - create a nonce
    // - capture that nonce, the original state, and the factory method (which takes the nonce + state) in a *new* state
    // - to make the factory method easier to consume, we define a different API surface to HybridCache
    //   which means we need to wrap the factory in one which essentially translates
    //   Func<Guid nonce, TState state, Ct> into Func<(Guid, TState), Ct>
    // - when GetOrCreateAsync returns, we check if the created nonce matches our scoped nonce
    // - if so, then we execute the tags factory and update the cache again with the tags, based on the loaded data
    public async ValueTask<TValue?> GetOrCreateAsync<TState, TValue>(
        string key,
        TState state,
        CacheFactory<TState, TValue> valueFactory,
        Func<TValue, IEnumerable<string>?> tagsFactory,
        HybridCacheEntryOptions? options = null,
        IEnumerable<string>? tags = null,
        CancellationToken cancellationToken = default
    )
    {
        var nonce = Guid.NewGuid();
        var capturedState = new CapturedState<TState, TValue>(state, nonce, valueFactory);

        var cached = await _cacheImpl.GetOrCreateAsync(
            key,
            capturedState,
            FactoryWrapper,
            options,
            tags,
            cancellationToken
        );

        if (cached is null)
            return default;

        if (cached.CacheNonce == nonce)
        {
            _metrics.OnCacheMiss<TValue>();
            var newTags = tagsFactory(cached.Value);
            if (newTags is not null)
                await _cacheImpl.SetAsync(key, cached, options, newTags, cancellationToken);
        }
        else
        {
            _metrics.OnCacheHit<TValue>();
        }

        return cached.Value;

        static ValueTask<Cached<TValue>?> FactoryWrapper(CapturedState<TState, TValue> capturedState, CancellationToken ct)
        {
            var originalState = capturedState.OriginalState;
            var nonce = capturedState.Nonce;
            var originalFactory = capturedState.OriginalFactory;

            return originalFactory(originalState, nonce, ct);
        }
    }

    // Wrap HybridCache implementation

    public override ValueTask<T> GetOrCreateAsync<TState, T>(
        string key,
        TState state,
        Func<TState, CancellationToken, ValueTask<T>> factory,
        HybridCacheEntryOptions? options = null,
        IEnumerable<string>? tags = null,
        CancellationToken cancellationToken = default
    ) =>
        _cacheImpl.GetOrCreateAsync(key, state, factory, options, tags, cancellationToken);

    public override ValueTask SetAsync<T>(
        string key,
        T value,
        HybridCacheEntryOptions? options = null,
        IEnumerable<string>? tags = null,
        CancellationToken cancellationToken = default
    ) =>
        _cacheImpl.SetAsync(key, value, options, tags, cancellationToken);

    public override ValueTask RemoveAsync(string key, CancellationToken cancellationToken = default) =>
        _cacheImpl.RemoveAsync(key, cancellationToken);

    public override ValueTask RemoveAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default) =>
        _cacheImpl.RemoveAsync(keys, cancellationToken);

    public override ValueTask RemoveByTagAsync(IEnumerable<string> tags, CancellationToken cancellationToken = default) =>
        _cacheImpl.RemoveByTagAsync(tags, cancellationToken);

    public override ValueTask RemoveByTagAsync(string tag, CancellationToken cancellationToken = default) =>
        _cacheImpl.RemoveByTagAsync(tag, cancellationToken);
}
