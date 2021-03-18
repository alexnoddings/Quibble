using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Quibble.Client.Services;

namespace Quibble.Client.Pages.Authentication
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
