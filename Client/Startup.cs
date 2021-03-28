using System;
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
            var baseAddress = _environment.BaseAddress;
            if (!baseAddress.EndsWith('/'))
                baseAddress += '/';

            services.AddOptions();
            services.AddOptions<JsonSerializerOptions>().Configure(o => o.PropertyNameCaseInsensitive = true);

            services.AddAuthorizationCore();
            services.AddScoped<IIdentityAuthenticationService, IdentityAuthenticationService>();
            services.AddHttpClient(nameof(IdentityAuthenticationService), httpClient => httpClient.BaseAddress = new Uri(baseAddress + "api/Authentication/"));
            services.AddScoped<IdentityAuthenticationStateProvider>();
            services.AddScoped<AuthenticationStateProvider>(serviceProvider => serviceProvider.GetRequiredService<IdentityAuthenticationStateProvider>());

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
    }
}
