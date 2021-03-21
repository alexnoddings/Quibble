using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Quibble.Client.Services;
using Quibble.Shared.Authentication;

namespace Quibble.Client.Pages.Settings
{
    public partial class ChangePassword
    {
        [Inject]
        private IdentityAuthenticationStateProvider AuthenticationProvider { get; set; } = default!;

        private class ChangePasswordModel : ChangePasswordRequest
        {
            [Required]
            [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        private ChangePasswordModel Model { get; } = new();

        private List<string>? Errors { get; set; }

        private bool WasSuccessful { get; set; }

        private async Task ChangePasswordAsync()
        {
            var result = await AuthenticationProvider.ChangePasswordAsync(Model.CurrentPassword, Model.NewPassword);
            WasSuccessful = result.WasSuccessful;
            if (WasSuccessful)
                Errors = null;
            else
                Errors = result.Errors?.ToList() ?? new List<string>();
        }
    }
}
