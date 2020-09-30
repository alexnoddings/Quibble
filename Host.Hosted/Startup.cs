using System;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quibble.Host.Common.Data;
using Quibble.Host.Common.Data.Entities;
using Quibble.Host.Common.Extensions;
using Quibble.Host.Hosted.Areas.Identity;
using Quibble.Host.Hosted.Data;
using Quibble.Host.Hosted.Platform;
using Quibble.Host.Hosted.Platform.Events;
using Quibble.Host.Hosted.Platform.Services;
using Quibble.UI.Core.Events;
using Quibble.UI.Core.Services;

namespace Quibble.Host.Hosted
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDbContext<QuibbleServerSideDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<DbQuibbleUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<QuibbleServerSideDbContext>();

            services.AddRazorPages();

            services.AddServerSideBlazor();

            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<DbQuibbleUser>>();

            services
                .AddBlazorise(options => options.ChangeTextOnKeyPress = true)
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();

            services.AddScoped<ISynchronisedQuizFactory, SynchronisedQuizFactory>();
            services.AddUserContextAccessor();
            services.AddQuibbleEntityFrameworkRepositories();
            services.AddScoped<IQuibbleDbContext, QuibbleServerSideDbContext>();

            AddPlatformServices(services);
        }

        private static void AddPlatformServices(IServiceCollection services)
        {
            services.AddScoped<ILoginHandler, HostedLoginHandler>();
            services.AddScoped<IAppMetadata, HostedAppMetadata>();

            services.AddScoped<IQuizService, HostedQuizService>();
            services.AddScoped<IQuizEvents, StaticQuizEvents>();
            services.AddScoped<IQuizEventsInvoker, StaticQuizEvents>();

            services.AddScoped<IRoundService, HostedRoundService>();
            services.AddScoped<IRoundEvents, StaticRoundEvents>();
            services.AddScoped<IRoundEventsInvoker, StaticRoundEvents>();

            services.AddScoped<IQuestionService, HostedQuestionService>();
            services.AddScoped<IQuestionEvents, StaticQuestionEvents>();
            services.AddScoped<IQuestionEventsInvoker, StaticQuestionEvents>();
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.ApplicationServices
              .UseBootstrapProviders()
              .UseFontAwesomeIcons();

            app.UseQuibbleContentRewriter();

            var rewriteOptions = new RewriteOptions();
            rewriteOptions.AddRedirect("^css/styles.css", "_content/Quibble.Host.Hosted/_framework/scoped.styles.css");
            app.UseRewriter(rewriteOptions);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/Index");
            });
        }
    }
}