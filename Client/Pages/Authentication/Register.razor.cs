using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BlazorIdentityBase.Client.Extensions;
using BlazorIdentityBase.Client.Services;
using BlazorIdentityBase.Shared.Authentication;
using Microsoft.AspNetCore.Components;

namespace BlazorIdentityBase.Client.Pages.Authentication
{
    public partial class Register
    {
        [Inject]
        private IdentityAuthenticationStateProvider AuthenticationProvider { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        private class RegisterModel : RegisterRequest
        {
            [Required]
            [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; }
        }

        private RegisterModel Model { get; } = new();

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

        private async Task RegisterAsync()
        {
            IsSubmitting = true;

            var result = await AuthenticationProvider.RegisterAsync(Model.UserName, Model.Email, Model.Password);
            if (!result.WasSuccessful)
                Errors = result.Errors?.ToList() ?? new List<string>();

            IsSubmitting = false;
        }
    }
}
