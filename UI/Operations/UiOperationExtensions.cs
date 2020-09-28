using System.Runtime.CompilerServices;

namespace Quibble.UI.Operations
{
    internal static class UiOperationExtensions
    {
        internal static TaskAwaiter GetAwaiter<TResult>(this UiOperation<TResult> uiOperation) => uiOperation.RunAsync().GetAwaiter();
    }
}
