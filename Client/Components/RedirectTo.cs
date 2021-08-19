using Microsoft.AspNetCore.Components;

namespace Quibble.Client.Components
{
    public class RedirectTo : ComponentBase, IDisposable
    {
        [Parameter]
        public string Url { get; set; } = string.Empty;

        [Parameter]
        public TimeSpan After { get; set; } = TimeSpan.Zero;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        protected bool IsDisposed { get; private set; }

        protected override async Task OnInitializedAsync()
        {
            if (IsDisposed)
                return;

            // Made local to prevent it changing while waiting
            string url = Url;
            if (string.IsNullOrEmpty(url))
                throw new InvalidOperationException($"Parameter {nameof(Url)} is null or empty. Use \"/\" to navigate to the root.");

            if (After > TimeSpan.Zero)
                await Task.Delay(After);

            NavigationManager.NavigateTo(url);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (IsDisposed) return;

            IsDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
