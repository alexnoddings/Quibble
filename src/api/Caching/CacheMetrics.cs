using System.Diagnostics.Metrics;

namespace Quibble.Caching;

public class CacheMetrics
{
    private readonly Counter<int> _counter;

    public CacheMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(Hosting.Metrics.MeterName);
        _counter = meter.CreateCounter<int>("quibble.cache.accessed");
    }

    public void OnCacheHit<T>()
    {
        var typeTag = GetTypeTag<T>();
        var accessTag = GetAccessTag(true);
        _counter.Add(1, typeTag, accessTag);
    }

    public void OnCacheMiss<T>()
    {
        var typeTag = GetTypeTag<T>();
        var accessTag = GetAccessTag(false);
        _counter.Add(1, typeTag, accessTag);
    }

    private static KeyValuePair<string, object?> GetTypeTag<T>()
    {
        const string typeKey = "quibble.cache.type";
        var typeName = typeof(T).Name;
        return new KeyValuePair<string, object?>(typeKey, typeName);
    }

    private static KeyValuePair<string, object?> GetAccessTag(bool hit)
    {
        const string typeKey = "quibble.cache.access";
        var hitStr = hit
            ? "hit"
            : "miss";
        return new KeyValuePair<string, object?>(typeKey, hitStr);
    }
}
