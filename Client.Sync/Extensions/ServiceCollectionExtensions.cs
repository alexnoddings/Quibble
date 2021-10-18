using Microsoft.Extensions.DependencyInjection;

namespace Quibble.Client.Sync.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSynchronisation(this IServiceCollection services)
        {
            services.AddTransient<ISyncedQuizFactory, SyncedQuizFactory>();
            return services;
        }
    }
}
