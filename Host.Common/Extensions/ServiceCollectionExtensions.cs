using Microsoft.Extensions.DependencyInjection;
using Quibble.Host.Common.Repositories;
using Quibble.Host.Common.Repositories.EntityFramework;
using Quibble.Host.Common.Services;
using Quibble.UI.Core.Services.Theme;

namespace Quibble.Host.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddQuibbleEntityFrameworkRepositories(this IServiceCollection services)
        {
            services.AddScoped<IQuizRepository, EfQuizRepository>();
            services.AddScoped<IRoundRepository, EfRoundRepository>();
            services.AddScoped<IQuestionRepository, EfQuestionRepository>();
            services.AddScoped<IParticipantRepository, EfParticipantRepository>();
            services.AddScoped<IUserRepository, EfUserRepository>();
            services.AddScoped<IAnswerRepository, EfAnswerRepository>();

            return services;
        }

        public static IServiceCollection AddUserContextAccessor(this IServiceCollection services)
        {
            services.AddScoped<IUserContextAccessor, EntityFrameworkUserContextAccessor>();

            return services;
        }

        public static IServiceCollection AddSimpleThemeProvider(this IServiceCollection services)
        {
            services.AddScoped<IThemeProvider, SimpleThemeProvider>();

            return services;
        }
    }
}
