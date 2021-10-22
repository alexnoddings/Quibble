using Microsoft.Extensions.DependencyInjection;
using Quibble.Client.Sync.Core.Factories;

namespace Quibble.Client.Sync.SignalR.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSignalrSynchronisation(this IServiceCollection services)
    {
        services.AddTransient<ISyncedQuizService, SignalrSyncedQuizService>();
        return services;
    }
}
