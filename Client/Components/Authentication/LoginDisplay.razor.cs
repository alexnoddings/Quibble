using System;
using BlazorIdentityBase.Client.Extensions;
using Microsoft.AspNetCore.Components;

namespace BlazorIdentityBase.Client.Components.Authentication
{
    public partial class LoginDisplay
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; }

        private void RedirectToLogin() => RedirectTo("login");
        private void RedirectToRegister() => RedirectTo("register");

        private void RedirectTo(string page)
        {
            var relativeUrl = NavigationManager.GetRelativeUrl();
            if (relativeUrl == string.Empty || !Uri.IsWellFormedUriString(relativeUrl, UriKind.Relative))
                relativeUrl = "/";
            NavigationManager.NavigateTo("/auth/" + page + "?returnUrl=" + Uri.EscapeDataString(relativeUrl));
        }
    }
}
