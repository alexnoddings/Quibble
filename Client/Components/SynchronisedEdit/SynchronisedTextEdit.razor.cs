using System;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Quibble.Client.Sync.DelayedExecution;

namespace Quibble.Client.Components.SynchronisedEdit
{
    public partial class SynchronisedTextEdit : IAsyncDisposable
    {
        [Parameter]
        public string Text { get; set; } = string.Empty;

        private string PreviousText { get; set; } = string.Empty;

        private string LocalText { get; set; } = string.Empty;

        [Parameter]
        public Func<string, Task> SaveFunction { get; set; } = default!;

        private Debouncer<string> SaveDebouncer { get; } = new(1000);

        [Parameter]
        public Func<string, Task>? PreviewFunction { get; set; }

        private Throttler<string> PreviewThrottler { get; } = new(333);

        [Parameter]
        public Size Size { get; set; } = Size.None;

        private bool IsSaveIconShown { get; set; }

        protected bool IsDisposed { get; private set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (PreviousText != Text)
            {
                PreviousText = Text;
                LocalText = Text;
            }

            SaveDebouncer.Executor = SaveAsync;
            PreviewThrottler.Executor = PreviewFunction;
        }

        private void OnTextChanged(string newText)
        {
            LocalText = newText;
            PreviewThrottler.Invoke(newText);
            SaveDebouncer.Invoke(newText);
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

        protected virtual async ValueTask DisposeAsync(bool isDisposing)
        {
            if (IsDisposed) return;

            if (isDisposing)
            {
                await PreviewThrottler.FlushAsync();
                await SaveDebouncer.FlushAsync();
            }

            IsDisposed = true;
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }
    }
}
