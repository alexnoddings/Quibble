using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Quibble.Server.Services.SendGrid
{
    /// <summary>
    /// Sends emails using <see href="https://github.com/sendgrid/sendgrid-csharp">SendGrid</see>.
    /// </summary>
    public class SendGridEmailSender : IAdvancedEmailSender
    {
        private readonly string _apiKey;
        private readonly string _domain;
        private readonly string _defaultFromUserName;
        private readonly string _defaultFromDisplayName;

        /// <summary>
        /// Initializes a new instance of <see cref="SendGridEmailSender"/>.
        /// </summary>
        /// <param name="options">The <see cref="IOptions{SendGridEmailOptions}"/>.</param>
        public SendGridEmailSender(IOptions<SendGridEmailOptions> options)
        {
            var emailOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));

            _apiKey = emailOptions.ApiKey ?? throw new ArgumentException($"{nameof(options)}.{nameof(SendGridEmailOptions.ApiKey)} cannot be null", nameof(options));
            _domain = emailOptions.Domain ?? throw new ArgumentException($"{nameof(options)}.{nameof(SendGridEmailOptions.Domain)} cannot be null", nameof(options));
            _defaultFromUserName = emailOptions.DefaultFromUserName ?? throw new ArgumentException($"{nameof(options)}.{nameof(SendGridEmailOptions.DefaultFromUserName)} cannot be null", nameof(options));
            _defaultFromDisplayName = emailOptions.DefaultFromDisplayName ?? throw new ArgumentException($"{nameof(options)}.{nameof(SendGridEmailOptions.DefaultFromDisplayName)} cannot be null", nameof(options));
        }

        /// <inheritdoc />
        public Task SendEmailAsync(string toAddress, string subject, string content) =>
            SendEmailCoreAsync(
                new EmailAddress(toAddress),
                CreateEmailAddress(_defaultFromUserName, _defaultFromDisplayName),
                subject,
                content
            );

        /// <inheritdoc />
        public Task SendEmailAsync(string toAddress, string toName, string subject, string content) =>
            SendEmailCoreAsync(
                new EmailAddress(toAddress, toName),
                CreateEmailAddress(_defaultFromUserName, _defaultFromDisplayName),
                subject,
                content
            );

        /// <inheritdoc />
        public Task SendEmailAsync(string toAddress, string toName, string fromUserName, string subject, string content) =>
            SendEmailCoreAsync(
                new EmailAddress(toAddress, toName),
                CreateEmailAddress(fromUserName, _defaultFromDisplayName),
                subject,
                content
            );

        /// <inheritdoc />
        public Task SendEmailAsync(string toAddress, string toName, string fromUserName, string fromDisplayName, string subject, string content) =>
            SendEmailCoreAsync(
                new EmailAddress(toAddress, toName),
                CreateEmailAddress(fromUserName, fromDisplayName),
                subject,
                content
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
            message.SetClickTracking(false, false);

            return client.SendEmailAsync(message);
        }
    }
}
