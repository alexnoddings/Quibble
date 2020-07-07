using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using Quibble.Common.SignalR;
using Quibble.Server.Data;
using Quibble.Server.Extensions.ServiceConfiguration.Email;
using Quibble.Server.Extensions.ServiceConfiguration.SignalR;
using Quibble.Server.Grpc;
using Quibble.Server.Hubs;
using Quibble.Server.Models.Users;

namespace Quibble.Server
{
    /// <summary>
    /// Handles the startup for the program.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initialises a new instance of <see cref="Startup"/>.
        /// </summary>
        /// <param name="configuration">The system configuration.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// The system configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configures the services added to the system's container.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCompression(options =>
            {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

            services.AddAuthentication()
                .AddIdentityServerJwt();

            services.AddSignalR()
                .AddJwtBearerAuthentication(SignalRPaths.HubsBase);

            services.AddSendGridEmail(options =>
            {
                options.ApiKey = Configuration["Email:Key"];
                options.Domain = Configuration["Email:Domain"];
                options.DefaultFromUserName = Configuration["Email:DefaultFromUserName"];
                options.DefaultFromDisplayName = Configuration["Email:DefaultFromDisplayName"];
            });

            services.AddGrpc();

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        /// <summary>
        /// Configures the system's request pipeline.
        /// </summary>
        /// <param name="app">The system's <see cref="IApplicationBuilder"/>.</param>
        /// <param name="env">The <see cref="IWebHostEnvironment"/>.</param>
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // Defaults to 30 days.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            // Use HTTP calls as a fallback if GRPC is not natively supported by the client.
            app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = false });

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapGrpcService<QuizService>().EnableGrpcWeb();
                endpoints.MapGrpcService<RoundService>().EnableGrpcWeb();
                endpoints.MapGrpcService<QuestionService>().EnableGrpcWeb();
                endpoints.MapHub<QuizHub>(SignalRPaths.QuizHub);
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
