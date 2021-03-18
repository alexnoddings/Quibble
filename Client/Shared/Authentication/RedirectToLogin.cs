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
            NavigationManager.NavigateTo("/auth/login?returnUrl=" + Uri.EscapeDataString(NavigationManager.GetRelativeUrl()));
    }
}
