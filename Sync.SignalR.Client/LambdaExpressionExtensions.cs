using Quibble.Sync.SignalR.Shared;
using System.Linq.Expressions;
using System.Reflection;

namespace Quibble.Sync.SignalR.Client;

internal static class LambdaExpressionExtensions
{
	public static string GetEventName(this Expression<Func<ISignalrEvents, Func<Task>>> eventMethodSelector) =>
		eventMethodSelector.GetMethodCallName();

	public static string GetEventName<T1>(this Expression<Func<ISignalrEvents, Func<T1, Task>>> eventMethodSelector) =>
		eventMethodSelector.GetMethodCallName();

	public static string GetEventName<T1, T2>(this Expression<Func<ISignalrEvents, Func<T1, T2, Task>>> eventMethodSelector) =>
		eventMethodSelector.GetMethodCallName();

	private static string GetMethodCallName(this LambdaExpression eventMethodSelector) =>
		TryGetMethodInfo(eventMethodSelector)?.Name
		?? throw new ArgumentException("Unsupported lambda.", nameof(eventMethodSelector));

	private static MethodInfo? TryGetMethodInfo(LambdaExpression eventMethodSelector)
	{
		if (eventMethodSelector.Body is not UnaryExpression unaryExpression) return null;
		if (unaryExpression.Operand is not MethodCallExpression methodCallExpression) return null;
		if (methodCallExpression.Object is not ConstantExpression constantExpression) return null;
		if (constantExpression.Value is not MethodInfo methodInfo) return null;

		return methodInfo;
	}
}
