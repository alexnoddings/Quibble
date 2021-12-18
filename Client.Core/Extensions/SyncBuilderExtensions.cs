using Microsoft.Extensions.DependencyInjection;
using Quibble.Client.Core.Factories;

namespace Quibble.Client.Core.Extensions;

public static class SyncBuilderExtensions
{
	public static ClientSyncBuilder AddClientSync(this IServiceCollection services)
	{
		services.AddTransient<ISyncedQuizFactory, SyncedQuizFactory>();
		return new ClientSyncBuilder(services);
	}
}
