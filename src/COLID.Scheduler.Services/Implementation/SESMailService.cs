using System;
using System.Collections.Generic;
using System.Text;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using COLID.Scheduler.Services.Interfaces;
using COLID.Scheduler.Services.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace COLID.Scheduler.Services.Implementation
{
    public class SESMailService : IMailService
    {
        private ILogger<SESMailService> _logger;
        private SmtpOptions _smtpOptions;
        public SESMailService(IOptionsSnapshot<SmtpOptions> smtpOptions, ILogger<SESMailService> logger)
        {
            this._smtpOptions = smtpOptions.Value;
            this._logger = logger;
        }

        public bool SendEmail(string subject, string body, string recipient)
        {
            try
            {
                using (var client = new AmazonSimpleEmailServiceClient(Amazon.RegionEndpoint.GetBySystemName(this._smtpOptions.AwsRegion)))
                {
                    var sendRequest = new SendEmailRequest
                    {
                        Source = _smtpOptions.Sender,
                        Destination = new Destination
                        {
                            ToAddresses = new List<string> { recipient }
                        },
                        Message = new Message
                        {
                            Subject = new Content(subject),
                            Body = new Body
                            {
                                Html = new Content
                                {
                                    Charset = "UTF-8",
                                    Data = body
                                },
                                Text = new Content
                                {
                                    Charset = "UTF-8",
                                    Data = body
                                }
                            }
                        }
                    };

                    var result = client.SendEmailAsync(sendRequest).Result;
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }
    }
}
