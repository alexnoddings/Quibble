using Microsoft.AspNetCore.Components;
using Quibble.Client.Demo.Services;

namespace Quibble.Client.Demo.Layouts;

public sealed partial class DemoLayout : IDisposable
{
	[Inject]
	private LayoutGutterFlag GutterFlag { get; set; } = default!;

	protected override void OnInitialized()
	{
		GutterFlag.OnUseGuttersChanged += OnUseGuttersChanged;
	}

	private Task OnUseGuttersChanged(bool _) => InvokeAsync(StateHasChanged);

	public void Dispose()
	{
		GutterFlag.OnUseGuttersChanged -= OnUseGuttersChanged;
	}
}
