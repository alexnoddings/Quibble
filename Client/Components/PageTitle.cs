using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web.Extensions.Head;

namespace Quibble.Client.Components
{
    // Both the Web Head and Blazorise packages both implement a Title component.
    // It is unreasonable to never import Blazorise and do Blazorise.ComponentName everywhere,
    // and the Razor compiler doesn't currently understand `@using Alias = Name.Space` directives.
    // As such, this component simply wraps the Head's Title component. This should be removed and
    // An alias should be used instead whenever this issue is fixed.
    // See https://github.com/dotnet/aspnetcore/issues/13090 for the issue tracker.
    public class PageTitle : ComponentBase
    {
        [Parameter] 
        public string Value { get; set; } = string.Empty;

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<Title>(0);
            builder.AddAttribute(1, "Value", Value);
            builder.CloseComponent();
        }
    }
}
