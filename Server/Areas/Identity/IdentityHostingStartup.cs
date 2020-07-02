using System;
using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Quibble.Server.Areas.Identity.IdentityHostingStartup))]
namespace Quibble.Server.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder = builder ?? throw new ArgumentNullException(nameof(builder));

            builder.ConfigureServices((context, services) => {
            });
        }
    }
}