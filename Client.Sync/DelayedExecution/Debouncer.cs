namespace Quibble.Client.Sync.DelayedExecution;

public class Debouncer<TValue> : DelayedExecutor<TValue>
{
    public Debouncer(double intervalMs)
        : base(intervalMs, true)
    {
    }
}
