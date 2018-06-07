using System;
using System.Net;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using Shared.Logging;

namespace Authorization.Providers
{
    public class EmailProvider
    {
        private const string CurrentClassName = nameof(EmailProvider);

        private readonly ILogger logger;

        private string SendGridApiKey => Properties.Settings.Default.SendGridApiKey;

        public EmailProvider(ILogger logger)
        {
            this.logger = logger;
        }

        public async Task SendAsync(string email, string subject, string content)
        {
            logger.Info(CurrentClassName, nameof(SendAsync), $"Sending email with content lenght '{content.Length}' with subject '{subject}' to email '{email}'");
            var msg = new SendGridMessage();
            msg.AddTo(email);
            msg.Subject = subject;
            msg.PlainTextContent = content;
            msg.From = new EmailAddress(Properties.Settings.Default.ServiceEmail);

            logger.Debug(CurrentClassName, nameof(SendAsync), $"Creating email provider");
            var provider = new SendGridClient(SendGridApiKey);
            logger.Debug(CurrentClassName, nameof(SendAsync), $"Email sending...");
            var response = await provider.SendEmailAsync(msg);
            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                var info = await response.Body.ReadAsStringAsync();
                throw new Exception(info);
            }
        }
    }
}