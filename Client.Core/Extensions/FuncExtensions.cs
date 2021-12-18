namespace Quibble.Client.Core.Extensions;

public static class FuncExtensions
{
	public static Task TryInvokeAsync(this Func<Task>? func) =>
		func?.Invoke() ?? Task.CompletedTask;

	public static Task TryInvokeAsync<T1>(this Func<T1, Task>? func, T1 arg1) =>
		func?.Invoke(arg1) ?? Task.CompletedTask;

	public static Task TryInvokeAsync<T1, T2>(this Func<T1, T2, Task>? func, T1 arg1, T2 arg2) =>
		func?.Invoke(arg1, arg2) ?? Task.CompletedTask;
}
