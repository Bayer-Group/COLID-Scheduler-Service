using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using COLID.Scheduler.Common.Constants;
using COLID.Scheduler.Common.DataModels;
using COLID.Scheduler.Services.Interface;
using COLID.SchedulerService.Jobs.Interface;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace COLID.SchedulerService.Jobs.Implementation
{
    public class MessageMailingJob : IMessageMailingJob
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IMailService _mail;
        private readonly ILogger<MessageMailingJob> _logger;
        private readonly IRemoteAppDataService _appDataService;

        public MessageMailingJob(IBackgroundJobClient backgroundJobClient, IMailService mail, ILogger<MessageMailingJob> logger, IRemoteAppDataService appDataService)
        {
            _backgroundJobClient = backgroundJobClient;
            _mail = mail;
            _logger = logger;
            _appDataService = appDataService;
        }

        [Queue(Queue.Beta)]
        public async Task ExecuteAsync(CancellationToken token)
        {
            var messages = await _appDataService.GetMessagesToSend();
            // Create jobs for single users
            if (messages != null && messages.Any())
            {
                foreach (var messagesGroupedByUsers in messages.GroupBy(x => x.UserEmail))
                {
                    // Messages need to be casted to ICollection object, because IEnumerable cannot be serialized.
                    _backgroundJobClient.Enqueue<IMessageMailingJob>(x =>
                        x.SendMessageForUser(messagesGroupedByUsers.Key, messagesGroupedByUsers.ToList()));
                }
            }
            _logger.LogInformation("Message Mailing Job Finished");
        }

        public void SendMessageForUser(string emailAddress, ICollection<Message> messages)
        {
            var emailBody = CreateEmailBody(messages);
            var messageIsSent = _mail.SendEmail("COLID: We have some messages for you", emailBody, emailAddress);

            if (messageIsSent)
            {
                _logger.LogInformation($"Message sent to {emailAddress}");
                _backgroundJobClient.Enqueue<IMessageMailingJob>(x => x.MarkMessagesAsSent(messages));
            }
        }

        public void MarkMessagesAsSent(ICollection<Message> messages)
        {
            foreach (var message in messages)
            {
                _appDataService.MarkMessageAsSent(message.UserId, message.Id);
            }
            var mailAddress = messages.First().UserEmail;
            _logger.LogInformation($"Messages for {mailAddress} marked as sent.");
        }

        private string CreateEmailBody(IEnumerable<Message> messages)
        {
            StringBuilder emailBody = new StringBuilder();

            emailBody.AppendFormat("Dear COLID user,");
            emailBody.AppendFormat("<br />");
            emailBody.AppendFormat("you will find all new messages below.");
            emailBody.AppendFormat("<br />");
            emailBody.AppendFormat("<br />");

            foreach (var message in messages)
            {
                emailBody.AppendFormat(message.Subject);
                emailBody.AppendFormat("<br />");
                emailBody.AppendFormat(message.Body);
                emailBody.AppendFormat("<hr />");
            }

            return emailBody.ToString();
        }
    }
}
