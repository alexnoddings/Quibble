using System;
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
        public EventCallback<string> ValueChanged { get; set; }

        [Parameter]
        public EventCallback<string> OnValueChanged { get; set; }

        [Parameter] 
        public bool FireOnInput { get; set; } = false;

        [Parameter]
        public bool FireOnBlur { get; set; } = true;

        protected override void OnInitialized()
        {
            if (!FireOnInput && !FireOnBlur)
                Logger.LogDebug($"Neither {nameof(FireOnInput)} nor {nameof(FireOnBlur)} are set. Parent component initialisation should be above this message.");
        }

        private async Task OnInput(ChangeEventArgs e)
        {
            Value = e.Value.ToString();
            await ValueChanged.InvokeAsync(Value ?? string.Empty).ConfigureAwait(false);

            if (!FireOnInput)
                return;

            await OnValueChanged.InvokeAsync(Value ?? string.Empty).ConfigureAwait(false);
        }

        private async Task OnBlur(FocusEventArgs _)
        {
            if (!FireOnBlur)
                return;

            await OnValueChanged.InvokeAsync(Value ?? string.Empty).ConfigureAwait(false);
        }
    }
}
