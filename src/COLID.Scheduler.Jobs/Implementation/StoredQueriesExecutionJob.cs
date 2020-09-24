using System;
using System.Threading;
using System.Threading.Tasks;
using COLID.Scheduler.Common.Constants;
using COLID.SchedulerService.Jobs.Interface;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace COLID.SchedulerService.Jobs.Implementation
{
    public class StoredQueriesExecutionJob : IStoredQueriesExecutionJob
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ILogger<StoredQueriesExecutionJob> _logger;

        public StoredQueriesExecutionJob(IBackgroundJobClient backgroundJobClient, ILogger<StoredQueriesExecutionJob> logger)
        {
            _backgroundJobClient = backgroundJobClient;
            _logger = logger;
        }

        [Queue(Queue.Beta)]
        public async Task ExecuteAsync(CancellationToken token)
        {
        }
    }
}
