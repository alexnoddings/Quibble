using System;
using System.Net.Http;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Quibble.Host.WASM.Client.Platform;
using Quibble.UI.Core.Services;

namespace Quibble.Host.WASM.Client
{
    /// <summary>
    /// Handles the startup for the program.
    /// </summary>
    public static class Startup
    {
        /// <summary>
        /// Configures the services added to the system's container.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure.</param>
        /// <param name="env">The <see cref="IWebAssemblyHostEnvironment"/> for the executing environment.</param>
        public static void ConfigureServices(IServiceCollection services, IWebAssemblyHostEnvironment env)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (env == null) throw new ArgumentNullException(nameof(env));

            services.AddHttpClient("Quibble.ServerAPI", client => client.BaseAddress = new Uri(env.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            // Supply HttpClient instances that include access tokens when making requests to the server project
            services.AddScoped(serviceProvider =>
                serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("Quibble.ServerAPI"));

            services.AddApiAuthorization();

            services
                .AddBlazorise(options => options.ChangeTextOnKeyPress = true)
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();

            // Platform-specific services
            services.AddScoped<ILoginHandler, WasmLoginHandler>();
            services.AddScoped<IAppMetadata, WasmAppMetadata>();
        }

        /// <summary>
        /// Configures the built <seealso cref="WebAssemblyHost"/>.
        /// </summary>
        /// <param name="host">The <see cref="WebAssemblyHost"/>.</param>
        public static void ConfigureHost(WebAssemblyHost host)
        {
            if (host == null) throw new ArgumentNullException(nameof(host));

            host.Services
                .UseBootstrapProviders()
                .UseFontAwesomeIcons();
        }
    }
}