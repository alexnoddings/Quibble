﻿using System;
using Microsoft.AspNetCore.Components;
using Quibble.Client.Extensions;

namespace Quibble.Client.Components.Authentication
{
    public partial class LoginDisplay
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        private void RedirectToLogin() => RedirectTo("login");
        private void RedirectToRegister() => RedirectTo("register");

        private void RedirectTo(string page)
        {
            var relativeUrl = NavigationManager.GetRelativeUrl();

            if (relativeUrl == string.Empty || !Uri.IsWellFormedUriString(relativeUrl, UriKind.Relative))
                relativeUrl = "/";

            if (relativeUrl == "logout")
                relativeUrl = "/";

            NavigationManager.NavigateTo("/" + page + "?returnUrl=" + Uri.EscapeDataString(relativeUrl));
        }
    }
}
