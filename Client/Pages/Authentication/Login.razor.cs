using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorIdentityBase.Client.Extensions;
using BlazorIdentityBase.Client.Services;
using BlazorIdentityBase.Shared.Authentication;
using Microsoft.AspNetCore.Components;

namespace BlazorIdentityBase.Client.Pages.Authentication
{
    public partial class Login
    {
        [Inject]
        private IdentityAuthenticationStateProvider AuthenticationProvider { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        private class LoginModel : LoginRequest
        {
        }

        private LoginModel Model { get; } = new();

        private IList<string>? Errors { get; set; }

        private string ReturnUrl { get; set; }

        private bool IsSubmitting { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            ReturnUrl = NavigationManager.GetQueryParameter("returnUrl", "/");

            // Only return to a well-formed relative url
            if (!Uri.IsWellFormedUriString(ReturnUrl, UriKind.Relative))
                ReturnUrl = "/";
        }

        private async Task LoginAsync()
        {
            IsSubmitting = true;

            var result = await AuthenticationProvider.LoginAsync(Model.UserName, Model.Password, Model.ShouldRememberUser);
            if (!result.WasSuccessful)
                Errors = result.Errors?.ToList() ?? new List<string>();

            IsSubmitting = false;
        }
    }
}
