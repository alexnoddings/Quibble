using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Quibble.Client.Sync.SignalR.Extensions
{
    internal static class LambdaExpressionExtensions
    {
        public static string GetMethodName(this LambdaExpression eventMethodSelector)
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
}
