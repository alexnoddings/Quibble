using System;
using System.Text.Json;
using Blazorise;
using Blazorise.Bootstrap;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Quibble.Client.Services.Authentication;
using Quibble.Client.Sync.Extensions;

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
            services.AddHttpClient(nameof(IdentityAuthenticationService), httpClient => httpClient.BaseAddress = new Uri(baseAddress + "Api/Authentication/"));
            services.AddScoped<IdentityAuthenticationStateProvider>();
            services.AddScoped<AuthenticationStateProvider>(serviceProvider => serviceProvider.GetRequiredService<IdentityAuthenticationStateProvider>());

            services.AddHttpClient("QuizApi", httpClient => httpClient.BaseAddress = new Uri(baseAddress + "Api/Quiz/"));
            services.AddSynchronisedQuizFactory();

            services
                .AddBlazorise(options =>
                {
                    options.ChangeTextOnKeyPress = true;
                    options.DelayTextOnKeyPress = true;
                    options.DelayTextOnKeyPressInterval = 80;
                })
                .AddBootstrapProviders();
        }
    }
}
