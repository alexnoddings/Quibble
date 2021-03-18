using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BlazorIdentityBase.Client.Services;
using BlazorIdentityBase.Shared.Authentication;
using Microsoft.AspNetCore.Components;

namespace BlazorIdentityBase.Client.Shared.Authentication.Profile
{
    public partial class RequestChangeEmail
    {
        [Inject]
        private IdentityAuthenticationStateProvider AuthenticationProvider { get; set; }

        private class RequestChangeEmailModel : RequestChangeEmailRequest
        {
        }

        private RequestChangeEmailModel Model { get; } = new();

        private IList<string>? Errors { get; set; }

        private bool WasSuccessful { get; set; }

        private string? CurrentEmail { get; set; }

        private bool IsSubmitting { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            CurrentEmail = (await AuthenticationProvider.GetAuthenticationStateAsync()).User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value;
        }

        private async Task RequestChangeEmailAsync()
        {
            IsSubmitting = true;

            var result = await AuthenticationProvider.RequestChangeEmailAsync(Model.Password, Model.NewEmail);
            WasSuccessful = result.WasSuccessful;
            if (WasSuccessful)
                Errors = null;
            else
                Errors = result.Errors?.ToList() ?? new List<string>();

            IsSubmitting = false;
        }
    }
}
