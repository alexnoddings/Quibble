﻿using System;
using System.Threading.Tasks;
using System.Windows.Input;
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

        protected override void OnInitialized()
        {
            base.OnInitialized();

            EnabledContent ??= ChildContent;

            if (EnabledContent == null) 
                throw new InvalidOperationException(nameof(EnabledContent));

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
                Disabled = false;
                // Force the state to acknowledge this not being disabled before throwing
                await InvokeAsync(StateHasChanged);
                throw;
            }

            Disabled = false;
        }
    }
}