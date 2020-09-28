using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Quibble.Core.Exceptions;

namespace Quibble.UI.Operations
{
    internal class UiOperation<TResult>
    {
        public UiOperationStatus Status { get; private set; } = UiOperationStatus.Uninitialised;

        public Task<TResult>? Task { get; private set; }

        public TResult Result
        {
            get
            {
                #pragma warning disable VSTHRD002 // Avoid problematic synchronous waits
                if (Task != null && Status == UiOperationStatus.Loaded)
                    return Task.Result;
                throw new InvalidOperationException("Operation is not loaded successfully.");
                #pragma warning restore VSTHRD002 // Avoid problematic synchronous waits
            }
        }

        [Pure]
        [MemberNotNullWhen(true, nameof(Task))]
        public bool IsSet => Task != null;

        public UiOperation()
        {
        }

        public UiOperation(Task<TResult>? task)
        {
            Task = task;
        }

        internal void Set(Task<TResult> task)
        {
            if (IsSet)
                throw new InvalidOperationException("Cannot set an operation more than once.");

            Task = task ?? throw new ArgumentNullException(nameof(task));
            Status = UiOperationStatus.Loading;
        }

        internal async Task RunAsync()
        {
            if (!IsSet)
                throw new InvalidOperationException("Cannot run an empty operation.");

            try
            {
                await Task;
            }
            catch (NotFoundException)
            {
                Status = UiOperationStatus.NotFound;
                return;
            }

            Status = UiOperationStatus.Loaded;
        }

        internal static UiOperation<TResult> Empty() => new UiOperation<TResult>();
    }
}
