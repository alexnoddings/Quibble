using System;
using Microsoft.AspNetCore.Components;
using Quibble.Client.Extensions;

namespace Quibble.Client.Components.Authentication
{
    public class RedirectToLogin : ComponentBase
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; }

        protected override void OnInitialized() =>
            NavigationManager.NavigateTo("/auth/login?returnUrl=" + Uri.EscapeDataString(NavigationManager.GetRelativeUrl()));
    }
}
