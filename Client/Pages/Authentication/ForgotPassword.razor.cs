using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorIdentityBase.Client.Services;
using BlazorIdentityBase.Shared.Authentication;
using Microsoft.AspNetCore.Components;

namespace BlazorIdentityBase.Client.Pages.Authentication
{
    public partial class ForgotPassword
    {
        [Inject]
        private IdentityAuthenticationStateProvider AuthenticationProvider { get; set; }

        private class ForgotPasswordModel : ForgotPasswordRequest
        {
        }

        private ForgotPasswordModel Model { get; } = new();

        private IList<string>? Errors { get; set; }

        private bool WasSuccessful { get; set; } = false;

        private bool IsSubmitting { get; set; }

        private async Task ForgotPasswordAsync()
        {
            IsSubmitting = true;

            var result = await AuthenticationProvider.ForgotPasswordAsync(Model.Email);
            if (result.WasSuccessful)
                WasSuccessful = true;
            else
                Errors = result.Errors?.ToList() ?? new List<string>();

            IsSubmitting = false;
        }
    }
}
