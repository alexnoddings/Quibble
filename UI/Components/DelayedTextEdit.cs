using System;
using System.Threading;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;

namespace Quibble.UI.Components
{
    public partial class DelayedTextEdit : TextEdit
    {
        private Task? _delayedTask;
        private CancellationTokenSource? _cts;

        [Parameter]
        public EventCallback<string> DelayedTextChanged { get; set; }

        [Parameter] 
        public TimeSpan Delay { get; set; } = TimeSpan.FromMilliseconds(500);

        protected override async Task OnInternalValueChanged(string value)
        {
            await base.OnInternalValueChanged(value);

            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            _delayedTask = Task.Delay(Delay, _cts.Token)
                .ContinueWith(CreateContinuationFunctionAsync,
                    cancellationToken: _cts.Token,
                    continuationOptions: TaskContinuationOptions.OnlyOnRanToCompletion,
                    scheduler: TaskScheduler.Current);
        }

        private Task CreateContinuationFunctionAsync(Task antecedent) =>
            InvokeAsync(() => DelayedTextChanged.InvokeAsync(Text));
    }
}
