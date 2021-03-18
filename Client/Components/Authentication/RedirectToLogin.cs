using System;
using System.Threading.Tasks;
using BlazorIdentityBase.Client.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorIdentityBase.Client.Components.Authentication
{
    public class RedirectToLogin : ComponentBase
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; }

        protected override void OnInitialized() =>
            NavigationManager.NavigateTo("/auth/login?returnUrl=" + Uri.EscapeDataString(NavigationManager.GetRelativeUrl()));
    }
}
