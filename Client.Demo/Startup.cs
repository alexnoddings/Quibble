using Blazored.LocalStorage;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Quibble.Client.Core.Extensions;
using Quibble.Client.Core.Services.Themeing;
using Quibble.Client.Demo.Services;
using Quibble.Server.Core;
using Quibble.Sync.InMemory;
using System.Text.Json;

namespace Quibble.Client.Demo;

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

		services
			.AddClientSync()
			.AddInMemory();
		services
			.AddServerSync()
			.AddInMemory();
		services.AddScoped<DemoQuizService>();

		services
			.AddBlazorise(options =>
			{
				options.ChangeTextOnKeyPress = true;
				options.DelayTextOnKeyPress = true;
				options.DelayTextOnKeyPressInterval = 80;
			})
			.AddBootstrapProviders()
			.AddFontAwesomeIcons();

		services.AddBlazoredLocalStorage();
		services.AddScoped<ThemeService>();
		services.AddSingleton<LayoutGutterFlag>();
	}
}
