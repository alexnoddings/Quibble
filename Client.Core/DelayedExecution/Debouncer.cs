namespace Quibble.Client.Core.DelayedExecution;

public class Debouncer<TValue> : DelayedExecutor<TValue>
{
	public Debouncer(double intervalMs)
		: base(intervalMs, true)
	{
	}
}
