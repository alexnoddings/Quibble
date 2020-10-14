using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Rewrite;

namespace Quibble.Host.Common.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        ///     Adds rewrites for Quibble content.
        ///     This <strong>must</strong> come before <c>app.UseStaticFiles()</c> so its rules can be executed before Blazors.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to add the rewrites to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseQuibbleContentRewriter(this IApplicationBuilder app)
        {
            var rwo = new RewriteOptions();

            string? thisAssemblyName = typeof(ApplicationBuilderExtensions).Assembly.GetName().Name;
            string? executingAssemblyName = Assembly.GetEntryAssembly()?.GetName()?.Name;

            rwo.AddRewrite("^favicon.ico", $"_content/{thisAssemblyName}/favicon.ico", false);
            rwo.AddRewrite("^css/site.css", $"_content/{thisAssemblyName}/css/site.css", false);
            rwo.AddRewrite("^img/bg.png", $"_content/{thisAssemblyName}/img/bg.png", false);

            if (executingAssemblyName != null)
                rwo.AddRewrite("^css/styles.css", $"{executingAssemblyName}.styles.css", false);

            // Allows for neater URLs and a more logical appearance to file structure
            // e.g. rewrite /css/blazorise.css to /_content/blazorise/blazorise.css
            rwo.AddRewrite("^(\\w+)/([\\w.]+).\\1", "_content/$2/$2.$1", false);

            // Scoped CSS uses a relative path for imports, but styles.css is located in /css/ so we need to drop the /css/ before the _content
            // e.g. rewrite /css/_content/Project/Project.bundle.scp.css to /_content/Project/Project.bundle.scp.css
            rwo.AddRewrite("^css/_content/([\\w.]+)/\\1.bundle.scp.css", "_content/$1/$1.bundle.scp.css", false);

            app.UseRewriter(rwo);

            return app;
        }
    }
}
