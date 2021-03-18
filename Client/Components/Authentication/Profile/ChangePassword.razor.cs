using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Quibble.Client.Services;
using Quibble.Shared.Authentication;

namespace Quibble.Client.Components.Authentication.Profile
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

        private IList<string>? Errors { get; set; }

        private bool WasSuccessful { get; set; }

        private bool IsSubmitting { get; set; }

        private async Task ChangePasswordAsync()
        {
            IsSubmitting = true;

            var result = await AuthenticationProvider.ChangePasswordAsync(Model.CurrentPassword, Model.NewPassword);
            WasSuccessful = result.WasSuccessful;
            if (WasSuccessful)
                Errors = null;
            else
                Errors = result.Errors?.ToList() ?? new List<string>();

            IsSubmitting = false;
        }
    }
}
