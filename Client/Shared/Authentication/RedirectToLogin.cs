using System;
using BlazorIdentityBase.Client.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorIdentityBase.Client.Shared.Authentication
{
    public class RedirectToLogin : ComponentBase
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        protected override void OnInitialized() =>
            // Using NavigateTo instead adds the current page to the history.
            // This means trying to go back will load this page and immediately navigate forwards again.
            JsRuntime.InvokeVoidAsync("blazorInterop.setLocation", "/auth/login?returnUrl=" + Uri.EscapeDataString(NavigationManager.GetRelativeUrl()));

    }
}
