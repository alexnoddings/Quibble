using Microsoft.AspNetCore.Components;
using Quibble.Client.Core.Extensions;

namespace Quibble.Client.App.Components.Authentication;

public partial class LoginDisplay
{
	[Inject]
	private NavigationManager NavigationManager { get; set; } = default!;

	private void RedirectToLogin() => RedirectTo("login");
	private void RedirectToRegister() => RedirectTo("register");

	private void RedirectTo(string page)
	{
		var relativeUrl = NavigationManager.GetRelativeUrl();

		if (string.IsNullOrEmpty(relativeUrl) || !Uri.IsWellFormedUriString(relativeUrl, UriKind.Relative))
			relativeUrl = "/";

		if (relativeUrl == "logout")
			relativeUrl = "/";

		NavigationManager.NavigateTo("/" + page + "?returnUrl=" + Uri.EscapeDataString(relativeUrl));
	}
}
