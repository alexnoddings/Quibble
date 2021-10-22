namespace Quibble.Client.Sync.Extensions;

public static class FuncExtensions
{
    public static Task InvokeAsync(this Func<Task>? func)
    {
        if (func is null)
            return Task.CompletedTask;

        return func.Invoke();
    }

    public static Task InvokeAsync<T>(this Func<T, Task>? func, T param)
    {
        if (func is null)
            return Task.CompletedTask;

        return func.Invoke(param);
    }
}
