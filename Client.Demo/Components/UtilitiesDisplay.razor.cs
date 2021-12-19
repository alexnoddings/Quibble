using Microsoft.AspNetCore.Components;
using Quibble.Client.Demo.Services;

namespace Quibble.Client.Demo.Components;

public partial class UtilitiesDisplay
{
	[Inject]
	private LayoutGutterFlag GutterFlag { get; set; } = default!;

	private Task ToggleGuttersAsync() => GutterFlag.SetUseGuttersAsync(!GutterFlag.UseGutters);
}
