using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web.Extensions.Head;

namespace Quibble.Client.Components
{
    public class PageTitle : ComponentBase
    {
        private const string TitleSuffix = "Quibble";
        private const string TitleBreak = " · ";

        [Parameter] 
        public string? Value { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            string pageTitle = 
                string.IsNullOrEmpty(Value) 
                    ? TitleSuffix 
                    : Value + TitleBreak + TitleSuffix;

            builder.OpenComponent<Title>(0);
            builder.AddAttribute(1, "Value", pageTitle);
            builder.CloseComponent();
        }
    }
}
