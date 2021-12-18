using Microsoft.Extensions.DependencyInjection;

namespace Quibble.Client.Core;

public class ClientSyncBuilder
{
	public IServiceCollection Services { get; }

	public ClientSyncBuilder(IServiceCollection services)
	{
		Services = services ?? throw new ArgumentNullException(nameof(services));
	}
}
