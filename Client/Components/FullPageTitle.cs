using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Quibble.Client.Components;

public class FullPageTitle : ComponentBase
{
    private const string TitleSuffix = "Quibble";
    private const string TitleBreak = " · ";

    [Parameter]
    public string? Value { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<PageTitle>(0);
        builder.AddAttribute(1, nameof(PageTitle.ChildContent), (RenderFragment)BuildFullPageTitleRenderTree);
        builder.CloseComponent();
    }

    private void BuildFullPageTitleRenderTree(RenderTreeBuilder builder)
    {
        string pageTitle =
            string.IsNullOrEmpty(Value)
                ? TitleSuffix
                : Value + TitleBreak + TitleSuffix;
        builder.AddContent(0, pageTitle);
    }
}
