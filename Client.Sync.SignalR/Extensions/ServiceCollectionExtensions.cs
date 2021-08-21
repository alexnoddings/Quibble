using Microsoft.Extensions.DependencyInjection;
using Quibble.Client.Sync.SignalR.Entities;

namespace Quibble.Client.Sync.SignalR.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSignalrSyncedQuizFactory(this IServiceCollection services) =>
            services.AddScoped<ISynchronisedQuizFactory, SignalrSyncedQuizFactory>();
    }
}
