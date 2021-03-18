using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Quibble.Client.Services;

namespace Quibble.Client.Pages.Authentication
{
    public partial class Logout
    {
        [Inject]
        private IdentityAuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            if (authState.User.Identity?.IsAuthenticated == true)
                await AuthenticationStateProvider.LogoutAsync();
        }
    }
}
