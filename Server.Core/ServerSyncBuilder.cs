using Microsoft.Extensions.DependencyInjection;

namespace Quibble.Server.Core;

public class ServerSyncBuilder
{
	public IServiceCollection Services { get; }

	public ServerSyncBuilder(IServiceCollection services)
	{
		Services = services ?? throw new ArgumentNullException(nameof(services));
	}
}
