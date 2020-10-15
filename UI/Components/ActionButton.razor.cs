using System;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;

namespace Quibble.UI.Components
{
    public partial class ActionButton : Button
    {
        [Parameter]
        public RenderFragment? EnabledContent { get; set; }

        [Parameter]
        public RenderFragment? DisabledContent { get; set; }

        [Parameter]
        public int? ReEnableDelayMs { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            EnabledContent ??= ChildContent;
            DisabledContent ??= EnabledContent;
        }

        private async Task OnClickAsync()
        {
            if (Disabled) return;
            if (!Clicked.HasDelegate) return;

            Disabled = true;

            try
            {
                await Clicked.InvokeAsync(this);
                if (Command?.CanExecute(CommandParameter) == true)
                    Command.Execute(CommandParameter);
            }
            catch (Exception)
            {
                await GetReEnableDelayAsync();
                Disabled = false;
                // Force the state to acknowledge this being re-enabled before throwing
                await InvokeAsync(StateHasChanged);
                throw;
            }

            await GetReEnableDelayAsync();
            Disabled = false;
        }

        private Task GetReEnableDelayAsync() =>
            ReEnableDelayMs != null && ReEnableDelayMs > 0
            ? Task.Delay(ReEnableDelayMs.Value)
            : Task.CompletedTask;
    }
}
