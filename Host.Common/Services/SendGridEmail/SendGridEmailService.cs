using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Quibble.Host.Common.Services.SendGridEmail
{
    /// <summary>
    /// Sends emails using <see href="https://github.com/sendgrid/sendgrid-csharp">SendGrid</see>.
    /// </summary>
    internal class SendGridEmailService : IEmailSender
    {
        private readonly string _apiKey;
        private readonly string _domain;
        private readonly EmailAddress _defaultEmailAddress;

        /// <summary>
        /// Initializes a new instance of <see cref="SendGridEmailService"/>.
        /// </summary>
        /// <param name="options">The <see cref="IOptions{SendGridEmailOptions}"/>.</param>
        public SendGridEmailService(IOptions<SendGridEmailOptions> options)
        {
            SendGridEmailOptions emailOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));

            _apiKey = Ensure.NotNullOrDefault(emailOptions.ApiKey, nameof(SendGridEmailOptions.ApiKey));
            _domain = Ensure.NotNullOrDefault(emailOptions.Domain, nameof(SendGridEmailOptions.Domain));
            string defaultFromUserName = Ensure.NotNullOrDefault(emailOptions.DefaultFromUserName, nameof(SendGridEmailOptions.DefaultFromUserName));
            string defaultFromDisplayName = Ensure.NotNullOrDefault(emailOptions.DefaultFromDisplayName, nameof(SendGridEmailOptions.DefaultFromDisplayName));

            _defaultEmailAddress = CreateEmailAddress(defaultFromUserName, defaultFromDisplayName);
        }

        /// <inheritdoc />
        public Task SendEmailAsync(string email, string subject, string htmlMessage) =>
            SendEmailCoreAsync(
                new EmailAddress(email),
                _defaultEmailAddress,
                subject,
                htmlMessage
            );

        private EmailAddress CreateEmailAddress(string userName, string? displayName) =>
            new EmailAddress($"{userName}@{_domain}", displayName ?? userName);

        private Task SendEmailCoreAsync(EmailAddress to, EmailAddress from, string subject, string content)
        {
            var client = new SendGridClient(_apiKey);

            var message = new SendGridMessage
            {
                From = from,
                Subject = subject,
                PlainTextContent = content,
                HtmlContent = content,
            };
            message.AddTo(to);

            return client.SendEmailAsync(message);
        }
    }
}