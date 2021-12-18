using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quibble.Client.Core;
using Quibble.Client.Core.Factories;
using Quibble.Server.Core;
using Quibble.Sync.InMemory.Server;

namespace Quibble.Sync.InMemory;

public static class SyncBuilderExtensions
{
	public static ClientSyncBuilder AddInMemory(this ClientSyncBuilder builder)
	{
		builder.Services.AddScoped<IInMemorySyncedQuizService, InMemorySyncedQuizService>();
		builder.Services.AddScoped<ISyncedQuizService>(services => services.GetRequiredService<IInMemorySyncedQuizService>());
		return builder;
	}

	public static ServerSyncBuilder AddInMemory(this ServerSyncBuilder builder)
	{
		builder.Services.AddSingleton<SyncContextStore>();
		builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("QuibbleInMemorySync"));
		return builder;
	}
}
