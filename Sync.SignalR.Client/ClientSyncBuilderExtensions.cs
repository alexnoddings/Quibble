using Microsoft.Extensions.DependencyInjection;
using Quibble.Client.Core;
using Quibble.Client.Core.Factories;

namespace Quibble.Sync.SignalR.Client;

public static class ClientSyncBuilderExtensions
{
	public static ClientSyncBuilder AddSignalr(this ClientSyncBuilder builder)
	{
		builder.Services.AddTransient<ISyncedQuizService, SignalrSyncedQuizService>();
		return builder;
	}
}
