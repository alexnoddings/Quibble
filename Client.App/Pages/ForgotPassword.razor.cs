using Microsoft.AspNetCore.Components;
using Quibble.Client.App.Services.Authentication;
using Quibble.Common.Authentication;

namespace Quibble.Client.App.Pages;

public partial class ForgotPassword
{
	[Inject]
	private IdentityAuthenticationStateProvider AuthenticationProvider { get; set; } = default!;

	private class ForgotPasswordModel : ForgotPasswordRequest
	{
	}

	private ForgotPasswordModel Model { get; } = new();

	private List<string>? Errors { get; set; }

	private bool WasSuccessful { get; set; } = false;

	private async Task ForgotPasswordAsync()
	{
		var result = await AuthenticationProvider.ForgotPasswordAsync(Model.Email);
		if (result.WasSuccessful)
			WasSuccessful = true;
		else
			Errors = result.Errors?.ToList() ?? new List<string>();
	}
}
