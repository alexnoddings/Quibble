using System.Threading.Tasks;
using BlazorIdentityBase.Client.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorIdentityBase.Client.Pages.Authentication
{
    public partial class Logout
    {
        [Inject]
        private IdentityAuthenticationStateProvider AuthenticationStateProvider { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            if (authState.User.Identity?.IsAuthenticated == true)
                await AuthenticationStateProvider.LogoutAsync();
        }
    }
}
