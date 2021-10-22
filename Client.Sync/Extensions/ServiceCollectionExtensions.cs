using Microsoft.Extensions.DependencyInjection;
using Quibble.Client.Sync.Core.Factories;

namespace Quibble.Client.Sync.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSynchronisation(this IServiceCollection services)
    {
        services.AddTransient<ISyncedQuizFactory, SyncedQuizFactory>();
        return services;
    }
}
