using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;

namespace Quibble.Client.Core.Components;

public class FullPageTitle : ComponentBase
{
	private const string DefaultTitleSuffix = "Quibble";
	private const string DefaultTitleBreak = " · ";

	private string TitleSuffix => Configuration.GetSection("Site").GetValue<string?>("TitleSuffix") ?? DefaultTitleSuffix;
	private string TitleBreak => Configuration.GetSection("Site").GetValue<string?>("TitleBreak") ?? DefaultTitleBreak;

	[Parameter]
	public string? Value { get; set; }

	[Inject]
	private IConfiguration Configuration { get; init; } = default!;

	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		builder.OpenComponent<PageTitle>(0);
		builder.AddAttribute(1, nameof(PageTitle.ChildContent), (RenderFragment)BuildFullPageTitleRenderTree);
		builder.CloseComponent();
	}

	private void BuildFullPageTitleRenderTree(RenderTreeBuilder builder)
	{
		var pageTitle =
			string.IsNullOrEmpty(Value)
				? TitleSuffix
				: Value + TitleBreak + TitleSuffix;
		builder.AddContent(0, pageTitle);
	}
}
