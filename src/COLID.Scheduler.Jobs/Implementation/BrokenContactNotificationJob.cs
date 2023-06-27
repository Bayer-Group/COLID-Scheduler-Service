using System.Threading;
using System.Threading.Tasks;
using COLID.Scheduler.Services.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;
using COLID.Scheduler.Common.Constants;
using COLID.Scheduler.Jobs.Interfaces;

namespace COLID.Scheduler.Jobs.Implementation
{
    public class BrokenContactNotificationJob : IBrokenContactNotificationJob
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ILogger<BrokenContactNotificationJob> _logger;
        private readonly IRemoteRegistrationService _registrationService;

        public BrokenContactNotificationJob(IBackgroundJobClient backgroundJobClient, ILogger<BrokenContactNotificationJob> logger, IRemoteRegistrationService registrationService)
        {
            _backgroundJobClient = backgroundJobClient;
            _logger = logger;
            _registrationService = registrationService;
        }

        [Queue(Queue.Beta)]
        public async Task ExecuteAsync(CancellationToken token)
        {
            _backgroundJobClient.Enqueue<IBrokenContactNotificationJob>(x => x.CheckDataStewardsAndDistributionEndpointContactsAndNotifyUsers());
            _logger.LogInformation("CheckInvalidContacts Job Finished");
        }

        public async Task CheckDataStewardsAndDistributionEndpointContactsAndNotifyUsers()
        {
            await _registrationService.CheckDataStewardsAndDistributionEndpointContactsAndNotifyUsers();
        }
    }
}
