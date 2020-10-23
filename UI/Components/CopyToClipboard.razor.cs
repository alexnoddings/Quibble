using System;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Quibble.UI.Components
{
    public sealed partial class CopyToClipboard
    {
        [Parameter]
        public string Content { get; set; } = string.Empty;

        [Inject]
        private IJSRuntime JSRuntime { get; init; } = default!;

        private IconName IconName { get; set; } = IconName.ShareAlt;

        protected override async Task OnInitializedAsync()
        {
            if (JSRuntime == null) throw new ArgumentException(nameof(JSRuntime));

            await base.OnInitializedAsync();
        }

        private async Task CopyToClipboardAsync()
        {
            await JSRuntime.InvokeVoidAsync("quibbleInterop.clipboard.write", Content);
            IconName = IconName.Check;
            await InvokeAsync(StateHasChanged);
            await Task.Delay(750);
            IconName = IconName.ShareAlt;
        }
    }
}
