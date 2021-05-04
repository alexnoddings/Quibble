using System;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Quibble.Client.Sync.DelayedExecution;

namespace Quibble.Client.Components
{
    public partial class SynchronisedTextEditor : IAsyncDisposable
    {
        [Parameter]
        public string Value { get; set; } = string.Empty;

        private string PreviousValue { get; set; } = string.Empty;

        private string LocalValue { get; set; } = string.Empty;

        [Parameter]
        public Func<string, Task> SaveFunction { get; set; } = default!;

        private Debouncer<string> SaveDebouncer { get; } = new(1000);

        [Parameter]
        public Func<string, Task>? PreviewFunction { get; set; }

        private Throttler<string> PreviewThrottler { get; } = new(333);

        [Parameter]
        public Size Size { get; set; } = Size.None;

        private bool IsSaveIconShown { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (PreviousValue != Value)
            {
                PreviousValue = Value;
                LocalValue = Value;
            }

            SaveDebouncer.Executor = SaveAsync;
            PreviewThrottler.Executor = PreviewFunction;
        }

        private void OnTextChanged(string newValue)
        {
            LocalValue = newValue;
            PreviewThrottler.Invoke(newValue);
            SaveDebouncer.Invoke(newValue);
        }

        private async Task OnBlurAsync()
        {
            await PreviewThrottler.FlushAsync();
            await SaveDebouncer.FlushAsync();
        }

        private Task SaveAsync(string newValue)
        {
            IsSaveIconShown = true;
            StateHasChanged();

            Task.Run(async () =>
            {
                await Task.Delay(900);
                IsSaveIconShown = false;
                return InvokeAsync(StateHasChanged);
            });
            return SaveFunction(newValue);
        }

        public async ValueTask DisposeAsync()
        {
            await PreviewThrottler.FlushAsync();
            await SaveDebouncer.FlushAsync();
        }
    }
}
