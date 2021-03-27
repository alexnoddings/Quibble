using System;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using Blazorise;
using Blazorise.Bootstrap;
using Fluxor;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Quibble.Client.Services.Authentication;

namespace Quibble.Client
{
    public class Startup
    {
        private readonly IWebAssemblyHostEnvironment _environment;

        public Startup(IWebAssemblyHostEnvironment environment)
        {
            _environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            ConfigureAuthorisation(services);
            ConfigureApi(services);

            services
                .AddBlazorise(options =>
                {
                    options.ChangeTextOnKeyPress = true;
                    options.DelayTextOnKeyPress = true;
                    options.DelayTextOnKeyPressInterval = 80;
                })
                .AddBootstrapProviders();

            services.AddFluxor(options =>
            {
                options.UseRouting();
                options.ScanAssemblies(Assembly.GetExecutingAssembly());

                if (_environment.IsDevelopment())
                    options.UseReduxDevTools();
            });
        }

        private void ConfigureAuthorisation(IServiceCollection services)
        {
            services.AddAuthorizationCore();
            services.AddScoped<IdentityAuthenticationStateProvider>();
            services.AddScoped<AuthenticationStateProvider>(serviceProvider => serviceProvider.GetRequiredService<IdentityAuthenticationStateProvider>());
            services.AddScoped<IIdentityAuthenticationService, IdentityAuthenticationService>();
        }

        private void ConfigureApi(IServiceCollection services)
        {
            services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(_environment.BaseAddress) });
            services.AddOptions<JsonSerializerOptions>().Configure(o => o.PropertyNameCaseInsensitive = true);
        }
    }
}
