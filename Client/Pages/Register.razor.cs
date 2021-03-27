using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Quibble.Client.Extensions;
using Quibble.Client.Services.Authentication;
using Quibble.Shared.Authentication;

namespace Quibble.Client.Pages
{
    public partial class Register
    {
        [Inject]
        private IdentityAuthenticationStateProvider AuthenticationProvider { get; set; } = default!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        private class RegisterModel : RegisterRequest
        {
            [Required]
            [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        private RegisterModel Model { get; } = new();

        private List<string>? Errors { get; set; }

        private string ReturnUrl { get; set; } = string.Empty;

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
