using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quibble.Client.Extensions.SignalR;
using Quibble.Client.Hubs;
using Quibble.Common;

namespace Quibble.Client
{
    /// <summary>
    /// Entry point for the program.
    /// </summary>
    public static class Program
    {
        internal const string ServerApiHttpClientName = "Quibble.ServerAPI";

        /// <summary>
        /// Entry point for the program.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        public static async Task Main(string[] args) =>
            await CreateHostBuilder(args).Build().RunAsync().ConfigureAwait(false);

        /// <summary>
        /// Creates a host builder to run the program.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        /// <returns>The <see cref="WebAssemblyHostBuilder"/> for the program.</returns>
        public static WebAssemblyHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            ConfigureServices(builder.Services, builder.HostEnvironment);
            return builder;
        }

        /// <summary>
        /// Configures the services added to the system's container.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure.</param>
        /// <param name="env">The <see cref="IWebAssemblyHostEnvironment"/> for the executing environment.</param>
        public static void ConfigureServices(IServiceCollection services, IWebAssemblyHostEnvironment env)
        {
            services.AddHttpClient(ServerApiHttpClientName, client => client.BaseAddress = new Uri(env.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            services.AddLogging(options =>
            {
                if (env.IsDevelopment())
                {
                    // Default log level
                    options.SetMinimumLevel(LogLevel.Debug);

                    options.AddFilter("Quibble", LogLevel.Trace);

                    options.AddFilter("Microsoft.AspNetCore", LogLevel.Debug);
                    options.AddFilter("Grpc.Net.Client", LogLevel.Debug);

                    options.AddFilter("Microsoft.Extensions.Http.DefaultHttpClientFactory", LogLevel.Information);

                    options.AddFilter("System.Net.Http.HttpClient", LogLevel.Warning);
                }
                else if (env.IsStaging())
                {
                    // Default log level
                    options.SetMinimumLevel(LogLevel.Debug);

                    options.AddFilter("Quibble", LogLevel.Debug);

                    options.AddFilter("Microsoft.AspNetCore", LogLevel.Information);
                    options.AddFilter("Grpc.Net.Client", LogLevel.Information);
                    options.AddFilter("Microsoft.Extensions.Http.DefaultHttpClientFactory", LogLevel.Information);

                    options.AddFilter("System.Net.Http.HttpClient", LogLevel.Warning);
                }
                else
                {
                    // Default log level
                    options.SetMinimumLevel(LogLevel.Information);

                    options.AddFilter("Quibble", LogLevel.Information);

                    options.AddFilter("Microsoft.AspNetCore", LogLevel.Information);
                    options.AddFilter("Grpc.Net.Client", LogLevel.Information);
                    options.AddFilter("Microsoft.Extensions.Http.DefaultHttpClientFactory", LogLevel.Information);

                    options.AddFilter("System.Net.Http.HttpClient", LogLevel.Warning);
                }
            });

            // Supply HttpClient instances that include access tokens when making requests to the server project
            services.AddTransient<HttpClient>(serviceProvider => 
                serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(ServerApiHttpClientName));

            services.AddHubConnection<QuizHubConnection>(SignalRPaths.QuizHub, innerHubConnection => new QuizHubConnection(innerHubConnection));

            services.AddApiAuthorization();
        }
    }
}
