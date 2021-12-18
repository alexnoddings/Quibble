namespace Quibble.Client.Core.DelayedExecution;

public class Throttler<TValue> : DelayedExecutor<TValue>
{
	public Throttler(double intervalMs)
		: base(intervalMs, false)
	{
	}
}
