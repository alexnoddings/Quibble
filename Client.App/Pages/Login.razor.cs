using Microsoft.AspNetCore.Components;
using Quibble.Client.App.Services.Authentication;
using Quibble.Client.Core.Extensions;
using Quibble.Common.Authentication;

namespace Quibble.Client.App.Pages;

public partial class Login
{
	[Inject]
	private IdentityAuthenticationStateProvider AuthenticationProvider { get; set; } = default!;

	[Inject]
	private NavigationManager NavigationManager { get; set; } = default!;

	private class LoginModel : LoginRequest
	{
	}

	private LoginModel Model { get; } = new();

	private List<string>? Errors { get; set; }

	private string ReturnUrl { get; set; } = string.Empty;

	protected override void OnInitialized()
	{
		base.OnInitialized();

		ReturnUrl = NavigationManager.GetQueryParameter("returnUrl", "/");

		// Only return to a well-formed relative url
		if (!Uri.IsWellFormedUriString(ReturnUrl, UriKind.Relative))
			ReturnUrl = "/";
	}

	private async Task LoginAsync()
	{
		var result = await AuthenticationProvider.LoginAsync(Model.UserName, Model.Password);
		if (!result.WasSuccessful)
			Errors = result.Errors?.ToList() ?? new List<string>();
	}
}
