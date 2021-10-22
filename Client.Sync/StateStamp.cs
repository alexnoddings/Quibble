using Quibble.Client.Sync.Core.Entities;

namespace Quibble.Client.Sync;

public static class StateStamp
{
    public static int ForProperties(params object?[] properties)
    {
        var hashCode = new HashCode();
        foreach (var property in properties)
            Add(ref hashCode, property);

        return hashCode.ToHashCode();
    }

    private static void Add(ref HashCode hashCode, object? obj)
    {
        if (obj is null)
            return;

        if (obj is ISyncedEntity syncedEntity)
        {
            // Let synced entities add their own state stamp
            hashCode.Add(syncedEntity.GetStateStamp());
        }
        else if (obj is List<object?> list)
        {
            // Check for list first as they're quicker to iterate than IEnumerables
            // Lists don't have their contents checked when added, need to add their items instead
            foreach (var listObj in list)
            {
                Add(ref hashCode, listObj);
            }
        }
        else if (obj is IEnumerable<object?> enumerable)
        {
            // Enumerables don't have their contents checked when added, need to add their items instead
            foreach (var enumerableObj in enumerable)
            {
                Add(ref hashCode, enumerableObj);
            }
        }
        else
        {
            // If it isn't enumerable, just add it normally
            hashCode.Add(obj);
        }
    }
}
