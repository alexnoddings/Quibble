using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quibble.Client.Extensions.ServiceConfiguration;
using Quibble.Common.Protos;

namespace Quibble.Client
{
    public static class Program
    {
        internal const string ServerApiHttpClientName = "Quibble.ServerAPI";

        public static async Task Main(string[] args) =>
            await CreateHostBuilder(args).Build().RunAsync().ConfigureAwait(false);

        public static WebAssemblyHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            ConfigureServices(builder.Services, builder.HostEnvironment);
            return builder;
        }

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
            services.AddTransient<HttpClient>(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(ServerApiHttpClientName));

            services.AddApiAuthorization();

            services.AddGrpcWebChannel();
            services.AddAuthorisedGrpcClient<QuizService.QuizServiceClient>();
        }
    }
}
