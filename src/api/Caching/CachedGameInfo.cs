using System.ComponentModel;

namespace Quibble.Caching;

/// <typeparam name="T">
///     The cached value - must be immutable (via <see langword="sealed"/> and <see cref="ImmutableObjectAttribute"/>).
/// </typeparam>
[ImmutableObject(true)]
public sealed class Cached<T> : ICached
{
    public required Guid CacheNonce { get; init; }
    public required T Value { get; init; }
}
