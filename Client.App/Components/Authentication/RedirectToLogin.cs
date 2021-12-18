using Microsoft.AspNetCore.Components;
using Quibble.Client.Core.Extensions;

namespace Quibble.Client.App.Components.Authentication;

public class RedirectToLogin : ComponentBase
{
	[Inject]
	private NavigationManager NavigationManager { get; init; } = default!;

	protected override void OnInitialized() =>
		NavigationManager.NavigateTo("/login?returnUrl=" + Uri.EscapeDataString(NavigationManager.GetRelativeUrl()));
}
