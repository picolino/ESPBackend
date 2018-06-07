using System;
using System.Net;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Authorization.Providers
{
    public class EmailProvider
    {
        private string SendGridApiKey => Properties.Settings.Default.SendGridApiKey;

        public async Task SendAsync(string email, string subject, string content)
        {
            var msg = new SendGridMessage();
            msg.AddTo(email);
            msg.Subject = subject;
            msg.PlainTextContent = content;
            msg.From = new EmailAddress(Properties.Settings.Default.ServiceEmail);

            var provider = new SendGridClient(SendGridApiKey);
            var response = await provider.SendEmailAsync(msg);
            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                var info = await response.Body.ReadAsStringAsync();
                throw new Exception(info);
            }
        }
    }
}