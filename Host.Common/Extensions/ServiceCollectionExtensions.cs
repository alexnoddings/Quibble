using System;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Quibble.Host.Common.Repositories;
using Quibble.Host.Common.Repositories.EntityFramework;
using Quibble.Host.Common.Services.SendGridEmail;
using Quibble.Host.Common.Services.UserContextAccessor;

namespace Quibble.Host.Common.Extensions
{
    /// <summary>
    /// Extension methods for adding various services to an <see cref="IServiceCollection" />.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds <c>IRepository</c>s to an <see cref="IServiceCollection"/> which use Entity Framework.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddQuibbleEntityFrameworkRepositories(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<IQuizRepository, EfQuizRepository>();
            services.AddScoped<IRoundRepository, EfRoundRepository>();
            services.AddScoped<IQuestionRepository, EfQuestionRepository>();
            services.AddScoped<IParticipantRepository, EfParticipantRepository>();
            services.AddScoped<IUserRepository, EfUserRepository>();
            services.AddScoped<IAnswerRepository, EfAnswerRepository>();

            return services;
        }

        /// <summary>
        /// Adds an <see cref="IUserContextAccessor"/> to an <see cref="IServiceCollection"/> which uses Entity Framework.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddEntityFrameworkUserContextAccessor(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<IUserContextAccessor, EntityFrameworkUserContextAccessor>();

            return services;
        }

        /// <summary>
        /// Adds an <see cref="IEmailSender"/> to an <see cref="IServiceCollection"/> which uses <see href="https://github.com/sendgrid/sendgrid-csharp">SendGrid</see>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configureOptions">A delegate to configure the <see cref="SendGridEmailOptions"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddSendGridEmail(this IServiceCollection services, Action<SendGridEmailOptions> configureOptions)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            services.Configure(configureOptions);
            services.AddTransient<IEmailSender, SendGridEmailService>();

            return services;
        }
    }
}
