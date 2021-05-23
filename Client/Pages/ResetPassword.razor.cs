using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Quibble.Client.Extensions;
using Quibble.Client.Services.Authentication;
using Quibble.Shared.Api.Authentication;

namespace Quibble.Client.Pages
{
    public partial class ResetPassword
    {
        [Inject]
        private IdentityAuthenticationStateProvider AuthenticationProvider { get; set; } = default!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        private class ResetPasswordModel : ResetPasswordRequest
        {
            [Required]
            [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        private ResetPasswordModel Model { get; } = new();

        private List<string>? Errors { get; set; }

        private bool IsTokenPreFilled { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Model.Email = NavigationManager.GetQueryParameter("email", string.Empty);
            Model.Token = NavigationManager.GetQueryParameter("token", string.Empty, unEscape: true);
            IsTokenPreFilled = !string.IsNullOrWhiteSpace(Model.Token);
        }

        private async Task ResetPasswordAsync()
        {
            var result = await AuthenticationProvider.ResetPasswordAsync(Model.Email, Model.Token, Model.NewPassword);
            if (result.WasSuccessful)
                NavigationManager.NavigateTo("/");
            else
                Errors = result.Errors?.ToList() ?? new List<string>();
        }
    }
}
