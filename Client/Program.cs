using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Quibble.Client.Services;

namespace Quibble.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<IdentityAuthenticationStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(services => services.GetRequiredService<IdentityAuthenticationStateProvider>());
            builder.Services.AddScoped<IIdentityAuthenticationService, IdentityAuthenticationService>();

            builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddOptions<JsonSerializerOptions>().Configure(o => o.PropertyNameCaseInsensitive = true);

            await builder.Build().RunAsync();
        }
    }
}
