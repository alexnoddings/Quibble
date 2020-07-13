using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Quibble.Client.Components
{
    public partial class ActionButton
    {
        [Parameter]
        public Func<Task>? Action { get; set; }

        [Parameter]
        public RenderFragment? Enabled { get; set; }

        [Parameter]
        public RenderFragment? Disabled { get; set; }

        [Parameter]
        public string Class { get; set; } = string.Empty;

        private bool IsDisabled { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (Enabled == null) throw new InvalidOperationException(nameof(Enabled));

            Disabled ??= Enabled;
        }

        private async Task RunAsync()
        {
            if (Action == null) return;

            if (IsDisabled) return;

            IsDisabled = true;

            try
            {
                await Action().ConfigureAwait(false);
            }
            catch (Exception)
            {
                IsDisabled = false;
                // Ensure the component is updated before the exception is thrown
                await InvokeAsync(StateHasChanged).ConfigureAwait(false);
                throw;
            }

            IsDisabled = false;
        }
    }
}
