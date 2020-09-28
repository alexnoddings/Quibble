using System;
using Microsoft.AspNetCore.Hosting;
using Quibble.UI.Identity.Areas.Identity;

[assembly: HostingStartup(typeof(IdentityHostingStartup))]
namespace Quibble.UI.Identity.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.ConfigureServices((context, services) => {
            });
        }
    }
}