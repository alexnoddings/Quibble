using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Quibble.Client.Demo.Services;
using Quibble.Common.Entities;
using Quibble.Server.Core;
using Quibble.Server.Core.Models;

namespace Quibble.Client.Demo;

public static class Program
{
	public static async Task Main(string[] args)
	{
		var builder = WebAssemblyHostBuilder.CreateDefault(args);

		builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

		builder.RootComponents.Add<HeadOutlet>("head::after");
		builder.RootComponents.Add<Application>("#app");

		var startup = new Startup(builder.HostEnvironment);
		startup.ConfigureServices(builder.Services);

		var host = builder.Build();

		var layoutGutterFlag = host.Services.GetRequiredService<LayoutGutterFlag>();
		await layoutGutterFlag.InitialiseAsync();

		await host.RunAsync();
	}
}
