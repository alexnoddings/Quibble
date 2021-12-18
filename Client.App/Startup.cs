using Blazored.LocalStorage;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Quibble.Client.App.Services.Authentication;
using Quibble.Client.App.Services.Quiz;
using Quibble.Client.Core.Extensions;
using Quibble.Client.Core.Services.Themeing;
using Quibble.Sync.SignalR.Client;
using System.Text.Json;

namespace Quibble.Client.App;

public class Startup
{
	private IWebAssemblyHostEnvironment Environment { get; }

	public Startup(IWebAssemblyHostEnvironment environment)
	{
		Environment = environment;
	}

	public void ConfigureServices(IServiceCollection services)
	{
		var baseAddress = Environment.BaseAddress;
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

		services.AddClientSync().AddSignalr();

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
		services.AddScoped<IQuizService, HttpQuizService>();
	}
}
