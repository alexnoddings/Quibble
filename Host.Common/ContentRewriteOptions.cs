using Microsoft.AspNetCore.Rewrite;

namespace Quibble.Host.Common
{
    public static class ContentRewriteOptions
    {
        public static RewriteOptions Create()
        {
            var rwo = new RewriteOptions();

            string? thisNameSpace = typeof(ContentRewriteOptions).Assembly.GetName().Name;
            rwo.AddRedirect("^favicon.ico", $"_content/{thisNameSpace}/favicon.ico");
            rwo.AddRedirect("^css/site.css", $"_content/{thisNameSpace}/css/site.css");
            rwo.AddRedirect("^img/bg.png", $"_content/{thisNameSpace}/img/bg.png");

            rwo.AddRedirect("^css/blazorise\\.css", "_content/Blazorise/blazorise.css");
            rwo.AddRedirect("^js/blazorise.js", "_content/Blazorise/blazorise.js");

            rwo.AddRedirect("^css/blazorise.bootstrap.css", "_content/Blazorise.Bootstrap/blazorise.bootstrap.css");
            rwo.AddRedirect("^js/blazorise.bootstrap.js", "_content/Blazorise.Bootstrap/blazorise.bootstrap.js");

            return rwo;
        }
    }
}
