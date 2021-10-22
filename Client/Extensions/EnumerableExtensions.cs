namespace Quibble.Client.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEqualityComparer<TSource>? comparer, params TSource[] second)
    {
        var set = new HashSet<TSource>(second, comparer);

        foreach (TSource element in first)
        {
            if (set.Add(element))
            {
                yield return element;
            }
        }
    }

    public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, params TSource[] second) => Except(first, null, second);
}
