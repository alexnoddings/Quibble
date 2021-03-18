using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorIdentityBase.Client.Services;
using BlazorIdentityBase.Shared.Authentication;
using Microsoft.AspNetCore.Components;

namespace BlazorIdentityBase.Client.Shared.Authentication.Profile
{
    public partial class ChangeUsername
    {
        [Inject]
        private IdentityAuthenticationStateProvider AuthenticationProvider { get; set; }

        private class ChangeUsernameModel : ChangeUsernameRequest
        {
        }

        private ChangeUsernameModel Model { get; } = new();

        private IList<string>? Errors { get; set; }

        private bool WasSuccessful { get; set; }

        private string CurrentUsername { get; set; }

        private bool IsSubmitting { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            CurrentUsername = (await AuthenticationProvider.GetAuthenticationStateAsync()).User?.Identity?.Name;
        }

        private async Task ChangeUsernameAsync()
        {
            IsSubmitting = true;

            var result = await AuthenticationProvider.ChangeUsernameAsync(Model.Password, Model.NewUsername);
            WasSuccessful = result.WasSuccessful;
            if (WasSuccessful)
                Errors = null;
            else
                Errors = result.Errors?.ToList() ?? new List<string>();

            IsSubmitting = false;
        }
    }
}
