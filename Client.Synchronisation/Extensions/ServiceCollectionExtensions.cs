using Microsoft.Extensions.DependencyInjection;
using Quibble.Client.Sync.Internal;

namespace Quibble.Client.Sync.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSynchronisedQuizFactory(this IServiceCollection services) =>
            services.AddScoped<ISynchronisedQuizFactory, SynchronisedQuizFactory>();
    }
}
