using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Quibble.Api;

internal static class ContextFactoryHelpers
{
    public static bool TryGetValue<T>(
        this RouteValueDictionary route,
        string key,
        [NotNullWhen(true)] out T? value
    ) where T : IParsable<T>
    {
        ArgumentNullException.ThrowIfNull(route);

        if (route.TryGetValue(key, out var obj)
            && obj is string str
            && T.TryParse(str, CultureInfo.InvariantCulture, out value))
        {
            return true;
        }

        value = default;
        return false;
    }
}
