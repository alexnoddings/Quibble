using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Quibble.Client.Services;

namespace Quibble.Client.Pages.Settings
{
    public partial class Profile
    {
        [Inject]
        private IdentityAuthenticationStateProvider AuthenticationProvider { get; set; } = default!;

        private string UserName { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            UserName = (await AuthenticationProvider.GetAuthenticationStateAsync()).User?.Identity?.Name ?? string.Empty;
        }
    }
}
