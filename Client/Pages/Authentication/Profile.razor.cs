using System.Threading.Tasks;
using BlazorIdentityBase.Client.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorIdentityBase.Client.Pages.Authentication
{
    public partial class Profile
    {
        [Inject]
        private IdentityAuthenticationStateProvider AuthenticationProvider { get; set; }

        private string UserName { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            UserName = (await AuthenticationProvider.GetAuthenticationStateAsync()).User?.Identity?.Name;
        }
    }
}
