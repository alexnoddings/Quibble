using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Quibble.Client.Components;

public class ConditionalTooltip : ComponentBase
{
    [Parameter]
    public bool Enabled { get; set; } = true;

    [Parameter]
    public string? Text { get; set; }

    [Parameter]
    public TooltipPlacement Placement { get; set; }

    [Parameter]
    public bool Multiline { get; set; }

    [Parameter]
    public bool AlwaysActive { get; set; }

    [Parameter]
    public bool ShowArrow { get; set; }

    [Parameter]
    public bool Inline { get; set; }

    [Parameter]
    public bool Fade { get; set; }

    [Parameter]
    public int FadeDuration { get; set; }

    [Parameter]
    public TooltipTrigger Trigger { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!Enabled)
        {
            builder.OpenElement(1, "div");
            builder.AddContent(2, ChildContent);
            builder.CloseElement();
        }
        else
        {
            // Because of how the tooltip is implemented, we can't just
            // override it and only conditionally call base.BuildRenderTree
            builder.OpenComponent<Tooltip>(3);
            builder.AddAttribute(4, nameof(Tooltip.Text), Text);
            builder.AddAttribute(4, nameof(Tooltip.Placement), Placement);
            builder.AddAttribute(4, nameof(Tooltip.Multiline), Multiline);
            builder.AddAttribute(4, nameof(Tooltip.AlwaysActive), AlwaysActive);
            builder.AddAttribute(4, nameof(Tooltip.ShowArrow), ShowArrow);
            builder.AddAttribute(4, nameof(Tooltip.Inline), Inline);
            builder.AddAttribute(4, nameof(Tooltip.Fade), Fade);
            builder.AddAttribute(4, nameof(Tooltip.FadeDuration), FadeDuration);
            builder.AddAttribute(4, nameof(Tooltip.Trigger), Trigger);
            builder.AddAttribute(4, nameof(Tooltip.ChildContent), ChildContent);
            builder.CloseComponent();
        }
    }
}
