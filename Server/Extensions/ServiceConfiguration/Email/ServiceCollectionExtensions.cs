using System;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Quibble.Server.Services.SendGrid;

namespace Quibble.Server.Extensions.ServiceConfiguration.Email
{
    /// <summary>
    /// Extensions methods for adding Email services in an <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a SendGrid email sender to an <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configureOptions">A delegate to configure the <see cref="SendGridEmailOptions"/>.</param>
        /// <returns>The same instance of the <see cref="IServiceCollection"/> for chaining.</returns>
        public static IServiceCollection AddSendGridEmail(this IServiceCollection serviceCollection, Action<SendGridEmailOptions> configureOptions)
        {
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            serviceCollection.Configure(configureOptions);
            // Still need to register IEmailSender as IdentityServer uses it
            serviceCollection.AddTransient<IEmailSender, SendGridEmailSender>();
            serviceCollection.AddTransient<IAdvancedEmailSender, SendGridEmailSender>();

            return serviceCollection;
        }
    }
}
