using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Quibble.Client.Extensions;
using Quibble.Client.Services.Authentication;
using Quibble.Shared.Authentication;

namespace Quibble.Client.Pages.Settings
{
    public partial class ChangeEmailConfirm
    {
        [Inject]
        private IdentityAuthenticationStateProvider AuthenticationProvider { get; set; } = default!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        private class ChangeEmailModel : ChangeEmailRequest
        {
        }

        private ChangeEmailModel Model { get; } = new();

        private IList<string>? Errors { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            Model.NewEmail = NavigationManager.GetQueryParameter("email", string.Empty);
            Model.Token = NavigationManager.GetQueryParameter("token", string.Empty, unEscape: true);

            var result = await AuthenticationProvider.ChangeEmailAsync(Model.NewEmail, Model.Token);
            if (result.WasSuccessful)
                NavigationManager.NavigateTo("/settings/email");
            else
                Errors = result.Errors?.ToList() ?? new List<string>();
        }
    }
}
