﻿using Microsoft.Extensions.DependencyInjection;
using Quibble.Host.Common.Repositories;
using Quibble.Host.Common.Repositories.EntityFramework;
using Quibble.Host.Common.Services;

namespace Quibble.Host.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddQuibbleEntityFrameworkRepositories(this IServiceCollection services)
        {
            services.AddScoped<IQuizRepository, EfQuizRepository>();
            services.AddScoped<IRoundRepository, EfRoundRepository>();
            services.AddScoped<IQuestionRepository, EfQuestionRepository>();

            return services;
        }

        public static IServiceCollection AddUserContextAccessor(this IServiceCollection services)
        {
            services.AddScoped<IUserContextAccessor, EntityFrameworkUserContextAccessor>();

            return services;
        }
    }
}