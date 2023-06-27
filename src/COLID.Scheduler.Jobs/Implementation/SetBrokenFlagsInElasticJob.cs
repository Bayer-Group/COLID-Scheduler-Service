using System.Threading;
using System.Threading.Tasks;
using COLID.Scheduler.Services.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;
using COLID.Scheduler.Common.Constants;
using COLID.Scheduler.Jobs.Interfaces;

namespace COLID.Scheduler.Jobs.Implementation
{
    public class SetBrokenFlagsInElasticJob : ISetBrokenFlagsInElasticJob
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ILogger<SetBrokenFlagsInElasticJob> _logger;
        private readonly IRemoteRegistrationService _registrationService;

        public SetBrokenFlagsInElasticJob(IBackgroundJobClient backgroundJobClient, ILogger<SetBrokenFlagsInElasticJob> logger, IRemoteRegistrationService registrationService)
        {
            _backgroundJobClient = backgroundJobClient;
            _logger = logger;
            _registrationService = registrationService;
        }

        [Queue(Queue.Beta)]
        public async Task ExecuteAsync(CancellationToken token)
        {
            _backgroundJobClient.Enqueue<ISetBrokenFlagsInElasticJob>(x => x.SetBrokenFlagsInElastic());
            _logger.LogInformation("SetBrokenFlagsInElastic Job Finished");
        }

        public async Task SetBrokenFlagsInElastic()
        {
            await _registrationService.SetBrokenFlagsInElastic();
        }
    }
}
