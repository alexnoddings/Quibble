using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quibble.Host.Common;
using Quibble.Host.Common.Data.Entities;
using Quibble.Host.WASM.Server.Data;

namespace Quibble.Host.WASM.Server
{
    public class WasmServerStartup : CommonStartup<QuibbleClientSideDbContext>
    {
        public WasmServerStartup(IConfiguration configuration)
            : base(configuration)
        {
        }

        protected override void ConfigurePlatformServices(IServiceCollection services)
        {
            services.AddIdentityServer()
                .AddApiAuthorization<DbQuibbleUser, QuibbleClientSideDbContext>();

            services.AddAuthentication()
                .AddIdentityServerJwt();

            services.AddControllersWithViews();
        }

        protected override void ConfigurePlatformApplication(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseBlazorFrameworkFiles();
            app.UseIdentityServer();
        }
    }
}
