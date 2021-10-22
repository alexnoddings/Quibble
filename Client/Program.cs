using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Quibble.Client;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

        builder.RootComponents.Add<HeadOutlet>("head::after");
        builder.RootComponents.Add<App>("#app");

        var startup = new Startup(builder.HostEnvironment);
        startup.ConfigureServices(builder.Services);

        await builder.Build().RunAsync();
    }
}
