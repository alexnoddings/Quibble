using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Quibble.Server.Services.SendGrid
{
    /// <summary>
    /// Provides more advanced functionality over an <see cref="IEmailSender"/>.
    /// </summary>
    public interface IAdvancedEmailSender : IEmailSender
    {
        /// <inheritdoc cref="SendEmailAsync(string,string,string,string,string,string)"/>
        Task SendEmailAsync(string toAddress, string toName, string subject, string content);

        /// <inheritdoc cref="SendEmailAsync(string,string,string,string,string,string)"/>
        Task SendEmailAsync(string toAddress, string toName, string fromUserName, string subject, string content);

        /// <summary>
        /// Sends an email.
        /// </summary>
        /// <param name="toAddress">The email address of the recipient.</param>
        /// <param name="toName">The name of the recipient.</param>
        /// <param name="fromUserName">The username part of the email address of the sender.</param>
        /// <param name="fromDisplayName">The display name of the sender.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="content">The content of the email.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SendEmailAsync(string toAddress, string toName, string fromUserName, string fromDisplayName, string subject, string content);
    }
}
