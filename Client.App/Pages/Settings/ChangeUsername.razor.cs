﻿using Microsoft.AspNetCore.Components;
using Quibble.Client.App.Services.Authentication;
using Quibble.Common.Authentication;

namespace Quibble.Client.App.Pages.Settings;

public partial class ChangeUsername
{
	[Inject]
	private IdentityAuthenticationStateProvider AuthenticationProvider { get; set; } = default!;

	private class ChangeUsernameModel : ChangeUsernameRequest
	{
	}

	private ChangeUsernameModel Model { get; } = new();

	private List<string>? Errors { get; set; }

	private bool WasSuccessful { get; set; }

	private string CurrentUsername { get; set; } = string.Empty;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();

		CurrentUsername = (await AuthenticationProvider.GetAuthenticationStateAsync()).User?.Identity?.Name ?? string.Empty;
	}

	private async Task ChangeUsernameAsync()
	{
		var result = await AuthenticationProvider.ChangeUsernameAsync(Model.Password, Model.NewUsername);
		WasSuccessful = result.WasSuccessful;
		if (WasSuccessful)
			Errors = null;
		else
			Errors = result.Errors?.ToList() ?? new List<string>();
	}
}
