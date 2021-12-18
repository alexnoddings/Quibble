using Microsoft.AspNetCore.Components;
using Quibble.Client.App.Services.Authentication;

namespace Quibble.Client.App.Pages;

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
