using Quibble.Shared.Sync.SignalR;
using System.Linq.Expressions;
using System.Reflection;

namespace Quibble.Client.Sync.SignalR.Extensions;

internal static class LambdaExpressionExtensions
{
    public static string GetEventName(this Expression<Func<ISignalrEvents, Func<Task>>> eventMethodSelector) =>
        GetMethodCallName(eventMethodSelector);

    public static string GetEventName<T1>(this Expression<Func<ISignalrEvents, Func<T1, Task>>> eventMethodSelector) =>
        GetMethodCallName(eventMethodSelector);

    public static string GetEventName<T1, T2>(this Expression<Func<ISignalrEvents, Func<T1, T2, Task>>> eventMethodSelector) =>
        GetMethodCallName(eventMethodSelector);

    private static string GetMethodCallName(this LambdaExpression eventMethodSelector)
    {
        var methodInfo = TryGetMethodInfo(eventMethodSelector);
        if (methodInfo is null)
            throw new ArgumentException("Unsupported lambda.", nameof(eventMethodSelector));

        return methodInfo.Name;
    }

    private static MethodInfo? TryGetMethodInfo(LambdaExpression eventMethodSelector)
    {
        if (eventMethodSelector.Body is not UnaryExpression unaryExpression) return null;
        if (unaryExpression.Operand is not MethodCallExpression methodCallExpression) return null;
        if (methodCallExpression.Object is not ConstantExpression constantExpression) return null;
        if (constantExpression.Value is not MethodInfo methodInfo) return null;

        return methodInfo;
    }
}
