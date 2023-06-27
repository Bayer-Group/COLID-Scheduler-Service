using System;
using System.Collections.Generic;
using System.Text;
using Hangfire;
using System.Threading;
using System.Threading.Tasks;
using COLID.SchedulerService.Jobs.Interfaces;
using Microsoft.Extensions.Logging;
using COLID.Scheduler.Common.Constants;
using COLID.Scheduler.Services.Interfaces;

namespace COLID.SchedulerService.Jobs.Implementation
{
    class BrokenEndpointNotificationJob : IBrokenEndpointNotificationJob
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ILogger<BrokenEndpointNotificationJob> _logger;
        private readonly IRemoteRegistrationService _registrationService;

        public BrokenEndpointNotificationJob(IBackgroundJobClient backgroundJobClient, ILogger<BrokenEndpointNotificationJob> logger, IRemoteRegistrationService registrationService)
        {
            _backgroundJobClient = backgroundJobClient;
            _logger = logger;
            _registrationService = registrationService;
        }

        [Queue(Queue.Beta)]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task ExecuteAsync(CancellationToken token)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            _backgroundJobClient.Enqueue<IBrokenEndpointNotificationJob>(x => x.NotifyUsersForInvalidDistributionEndpoints());
            _logger.LogInformation("InvalidDitributionEndpointNotification Job Finished");
        }

        public void NotifyUsersForInvalidDistributionEndpoints()
        {
            _logger.LogInformation("Calling registration api for to check the invalid distribution endpoint(s)");
            _registrationService.CheckDistributionEndpointValidityAndNotifyUsersAsync();
        }
    }
}
