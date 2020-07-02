﻿namespace Quibble.Server.Services.SendGrid
{
    /// <summary>
    /// Options for the SendGrid email sender.
    /// </summary>
    public class SendGridEmailOptions
    {
        /// <summary>
        /// Api Key used to send emails.
        /// </summary>
        public string? ApiKey { get; set; }

        /// <summary>
        /// The email domain to use for the sender.
        /// </summary>
        public string? Domain { get; set; }

        /// <summary>
        /// The default name to use for the sender.
        /// </summary>
        public string? DefaultFromDisplayName { get; set; }

        /// <summary>
        /// The default address to use for the sender.
        /// </summary>
        public string? DefaultFromUserName { get; set; }
    }
}