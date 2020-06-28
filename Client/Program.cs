using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Quibble.Client
{
    public static class Program
    {
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
            services.AddHttpClient("Quibble.ServerAPI", client => client.BaseAddress = new Uri(env.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            // Supply HttpClient instances that include access tokens when making requests to the server project
            services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Quibble.ServerAPI"));

            services.AddApiAuthorization();
        }
    }
}
