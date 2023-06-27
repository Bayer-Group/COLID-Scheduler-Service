using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using COLID.Scheduler.Services.Interfaces;
using COLID.Scheduler.Services.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace COLID.Scheduler.Services.Implementation
{
    public class MailService : IMailService
    {
        private ILogger<MailService> _logger;
        private SmtpOptions _smtpOptions;

        public MailService(IOptionsSnapshot<SmtpOptions> smtpOptions, ILogger<MailService> logger)
        {
            _smtpOptions = smtpOptions.Value;
            _logger = logger;
        }

        public bool SendEmail(string subject, string body, string recipient)
        {
            try
            {
                using (var client = new SmtpClient(_smtpOptions.Server, _smtpOptions.Port) { EnableSsl = _smtpOptions.EnableSsl })
                {
                    if (!(string.IsNullOrEmpty(_smtpOptions.User) && string.IsNullOrEmpty(_smtpOptions.Password)))
                    {
                        client.Credentials = new NetworkCredential(_smtpOptions.User, _smtpOptions.Password);
                    }

                    using var message = new MailMessage();
                    message.IsBodyHtml = true;
                    message.Subject = subject;
                    message.Body = body;
                    message.From = new MailAddress(_smtpOptions.Sender);
                    message.To.Add(recipient);

                    // Try to send the message.
                    try
                    {
                        client.Send(message);
                        return true;
                    }
                    catch (System.Exception ex)
                    {
                        _logger.LogError("An error occured during mail tansmission: ", ex);
                        return false;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Dictionary<string, object> additionalInfo = new Dictionary<string, object>();
                additionalInfo.Add("Subject", subject);
                additionalInfo.Add("From", _smtpOptions.Sender);
                additionalInfo.Add("To", recipient);
                additionalInfo.Add("Body", body);
                additionalInfo.Add("Error", ex.Message);
                _logger.LogError(additionalInfo.ToString());
            }
            return false;
        }
    }
}
