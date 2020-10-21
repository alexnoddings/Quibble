using System;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quibble.Host.Common.Data;
using Quibble.Host.Common.Data.Entities;
using Quibble.Host.Common.Extensions;
using Quibble.UI.Core.Services.Data;
using Quibble.UI.Core.Services.Theme;

namespace Quibble.Host.Common
{
    public abstract class CommonStartup<TDbContext> where TDbContext : DbContext, IQuibbleDbContext
    {
        protected IConfiguration Configuration { get; }

        protected CommonStartup(IConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        protected abstract void ConfigurePlatformServices(IServiceCollection services);

        public void ConfigureServices(IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddDbContext<TDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<DbQuibbleUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<TDbContext>();

            services.AddRazorPages();

            services
                .AddBlazorise(options => options.ChangeTextOnKeyPress = true)
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();

            services.AddScoped<ISynchronisedQuizFactory, SynchronisedQuizFactory>();
            services.AddEntityFrameworkUserContextAccessor();
            services.AddScoped<IThemeProvider, SimpleThemeProvider>();
            services.AddQuibbleEntityFrameworkRepositories();
            services.AddScoped<IQuibbleDbContext, TDbContext>();

            ConfigurePlatformServices(services);
        }

        protected abstract void ConfigurePlatformApplication(IApplicationBuilder app, IWebHostEnvironment env);

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            if (env == null) throw new ArgumentNullException(nameof(env));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseQuibbleContentRewriter();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.ApplicationServices
                .UseBootstrapProviders()
                .UseFontAwesomeIcons();

            ConfigurePlatformApplication(app, env);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/Index");
            });
        }
    }
}
