using System.Timers;
using Timer = System.Timers.Timer;

namespace Quibble.Client.Sync.DelayedExecution;

public class DelayedExecutor<TValue> : IDisposable
{
    private readonly Timer _timer;
    private readonly bool _shouldDebounce;
    private TValue? _value;

    public Func<TValue, Task>? Executor { get; set; }

    protected bool IsDisposed { get; private set; }

    public DelayedExecutor(double intervalMs, bool shouldDebounce)
    {
        _timer = new Timer(intervalMs) { AutoReset = false };
        _timer.Elapsed += OnTimerElapsed;

        _shouldDebounce = shouldDebounce;
    }

    public void Invoke(TValue value)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(DelayedExecutor<TValue>));

        if (_shouldDebounce)
            _timer.Stop();

        _value = value;
        _timer.Start();
    }

    public Task FlushAsync()
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(DelayedExecutor<TValue>));

        if (!_timer.Enabled)
            return Task.CompletedTask;

        _timer.Stop();

        if (_value is not null && Executor is not null)
            return Executor.Invoke(_value);

        return Task.CompletedTask;
    }

    private async void OnTimerElapsed(object? sender, ElapsedEventArgs args)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(DelayedExecutor<TValue>));

        if (_value is null || Executor is null)
            return;

        await Executor.Invoke(_value);
    }

    protected virtual void Dispose(bool isDisposing)
    {
        if (IsDisposed) return;

        if (isDisposing)
        {
            _timer.Stop();
            _timer.Dispose();
        }

        IsDisposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
