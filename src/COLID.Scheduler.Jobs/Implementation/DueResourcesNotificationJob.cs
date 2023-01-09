using System;
using System.Threading;
using System.Threading.Tasks;
using COLID.Scheduler.Common.Constants;
using COLID.Scheduler.Services.Interface;
using COLID.SchedulerService.Jobs.Interface;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace COLID.SchedulerService.Jobs.Implementation
{
    public class DueResourcesNotificationJob : IDueResourcesNotificationJob
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ILogger<MessageDeletionJob> _logger;
        private readonly IRemoteRegistrationService _registrationService;
        private readonly IMailService _mail;


        public DueResourcesNotificationJob(IBackgroundJobClient backgroundJobClient, IMailService mail, ILogger<MessageDeletionJob> logger, IRemoteRegistrationService registrationService)
        {
            _backgroundJobClient = backgroundJobClient;
            _logger = logger;
            _registrationService = registrationService;
            _mail = mail;
        }

        [Queue(Queue.Beta)]
        public async Task ExecuteAsync(CancellationToken token)
        {
            _backgroundJobClient.Enqueue<IDueResourcesNotificationJob>(x => x.NotifyDataStewardsForDueResourceReview());
            _logger.LogInformation("DueResourcesNotificationJob Job Finished");
        }

        public async Task NotifyDataStewardsForDueResourceReview()
        {
            var emailList = await _registrationService.NotifyDataStewardsForDueResourceReview();
            foreach(var emailToSend in emailList)
            {
                _mail.SendEmail("COLID: We have some messages for you", emailToSend.Value, emailToSend.Key);
            }
        }
    }
}
