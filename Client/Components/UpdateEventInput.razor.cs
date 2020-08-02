using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;

namespace Quibble.Client.Components
{
    public partial class UpdateEventInput
    {
        [Parameter]
        public string? Class { get; set; }

        [Parameter]
        public string? Label { get; set; } 

        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();

        [Parameter]
        public string? Value { get; set; }

        [Parameter]
        public string? Placeholder { get; set; }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        [Parameter]
        public EventCallback<string> OnValueChanged { get; set; }

        [Parameter] 
        public UpdateEvent FireEvent { get; set; } = UpdateEvent.OnBlur;

        private bool ShouldUpdateOnBlur => FireEvent.HasFlag(UpdateEvent.OnBlur);
        private bool ShouldUpdateOnInput => FireEvent.HasFlag(UpdateEvent.OnInput);

        protected override void OnInitialized()
        {
            if (!ShouldUpdateOnBlur && !ShouldUpdateOnInput)
                Logger.LogDebug($"Neither {nameof(UpdateEvent.OnBlur)} nor {nameof(UpdateEvent.OnInput)} are set. Parent component initialisation should be above this message.");
        }

        private async Task OnInputAsync(ChangeEventArgs e)
        {
            Value = e.Value.ToString();
            await ValueChanged.InvokeAsync(Value ?? string.Empty).ConfigureAwait(false);

            if (!ShouldUpdateOnInput)
                return;

            await OnValueChanged.InvokeAsync(Value ?? string.Empty).ConfigureAwait(false);
        }

        private async Task OnBlurAsync(FocusEventArgs _)
        {
            if (!ShouldUpdateOnBlur)
                return;

            await OnValueChanged.InvokeAsync(Value ?? string.Empty).ConfigureAwait(false);
        }
    }
}
