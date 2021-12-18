using Microsoft.AspNetCore.Components;
using Quibble.Client.Core.Services.Themeing;

namespace Quibble.Client.Core.Components;

public partial class ThemeDisplay
{
	[Inject]
	private ThemeService ThemeService { get; set; } = default!;
}
