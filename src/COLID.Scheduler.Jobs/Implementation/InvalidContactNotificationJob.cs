using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using COLID.Scheduler.Services.Interfaces;
using COLID.SchedulerService.Jobs.Implementation;
using COLID.SchedulerService.Jobs.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;
using COLID.Scheduler.Common.Constants;
using COLID.Scheduler.Jobs.Interfaces;

namespace COLID.Scheduler.Jobs.Implementation
{
    public class InvalidContactNotificationJob : IInvalidContactNotificationJob
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ILogger<InvalidContactNotificationJob> _logger;
        private readonly IRemoteRegistrationService _registrationService;

        public InvalidContactNotificationJob(IBackgroundJobClient backgroundJobClient, ILogger<InvalidContactNotificationJob> logger, IRemoteRegistrationService registrationService)
        {
            _backgroundJobClient = backgroundJobClient;
            _logger = logger;
            _registrationService = registrationService;
        }

        [Queue(Queue.Beta)]
        public async Task ExecuteAsync(CancellationToken token)
        {
            _backgroundJobClient.Enqueue<IInvalidContactNotificationJob>(x => x.CheckDataStewardsAndDistributionEndpointContactsAndNotifyUsers());
            _logger.LogInformation("CheckInvalidContacts Job Finished");
        }

        public async Task CheckDataStewardsAndDistributionEndpointContactsAndNotifyUsers()
        {
            await _registrationService.CheckDataStewardsAndDistributionEndpointContactsAndNotifyUsers();
        }
    }
}
