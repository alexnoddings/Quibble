using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Quibble.Client.Components
{
    public class RedirectTo : ComponentBase, IDisposable
    {
        [Parameter]
        public string Url { get; set; }

        [Parameter]
        public TimeSpan After { get; set; } = TimeSpan.Zero;

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        private bool _isDisposed = false;

        protected override async Task OnInitializedAsync()
        {
            // Made local to prevent it changing while waiting
            string url = Url;
            if (url is null)
                throw new InvalidOperationException($"Parameter {nameof(Url)} is not set.");

            if (After > TimeSpan.Zero)
                await Task.Delay(After);

            if (_isDisposed) 
                return;

            NavigationManager.NavigateTo(url);
        }

        public void Dispose()
        {
            _isDisposed = true;
        }
    }
}
