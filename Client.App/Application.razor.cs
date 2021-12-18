using Microsoft.AspNetCore.Components;
using Quibble.Client.Core.Services.Themeing;

namespace Quibble.Client.App;

public sealed partial class Application : IDisposable
{
	[Inject]
	private ThemeService ThemeService { get; set; } = default!;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();

		await ThemeService.InitialiseAsync();
		ThemeService.ThemeUpdated += OnThemeUpdatedAsync;
	}

	private Task OnThemeUpdatedAsync() => InvokeAsync(StateHasChanged);

	public void Dispose()
	{
		ThemeService.ThemeUpdated -= OnThemeUpdatedAsync;
	}
}
