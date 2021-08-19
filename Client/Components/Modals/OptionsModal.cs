using Microsoft.AspNetCore.Components;

namespace Quibble.Client.Components.Modals
{
    public class OptionsModal<TValue> : ComponentBase
    {
        [Parameter]
        public string Title { get; set; } = string.Empty;

        [CascadingParameter]
        public OptionsModalHost OptionsModalHost { get; set; } = default!;

        [Parameter]
        public RenderFragment Body { get; set; } = default!;

        [Parameter]
        public RenderFragment<OptionsModalContext<TValue?>> Footer { get; set; } = default!;

        [Parameter]
        public bool CanBeDismissed { get; set; } = true;

        public Task<TValue?> ShowAsync(TValue? dismissValue = default) =>
            OptionsModalHost.ShowAsync(Title, Body, Footer, CanBeDismissed, dismissValue);

        protected override bool ShouldRender() => false;
    }
}
