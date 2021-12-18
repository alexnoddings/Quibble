using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Quibble.Server.Core;

namespace Quibble.Sync.SignalR.Server;

public static class SignalrServerSyncExtensions
{
	public static ServerSyncBuilder AddSignalr(this ServerSyncBuilder builder)
	{
		builder.Services.AddSignalR();
		return builder;
	}

	public static IEndpointRouteBuilder MapSignalrSync(this IEndpointRouteBuilder endpoints)
	{
		endpoints.MapHub<SignalrSyncHub>("_sync/{QuizId:guid}");
		return endpoints;
	}
}
