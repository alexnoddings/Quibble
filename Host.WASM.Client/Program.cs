using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Quibble.Host.WASM.Client
{
    /// <summary>
    /// Entry point for the program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Entry point for the program.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        public static async Task Main(string[] args)
        {
            var hostBuilder = WebAssemblyHostBuilder.CreateDefault(args);
            hostBuilder.RootComponents.Add<Quibble.UI.App>("#app");
            WasmClientStartup.ConfigureServices(hostBuilder.Services, hostBuilder.HostEnvironment);

            await using var host = hostBuilder.Build();
            WasmClientStartup.ConfigureHost(host);

            await host.RunAsync().ConfigureAwait(false);
        }
    }
}