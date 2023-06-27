using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using COLID.Scheduler.Common.Constants;
using COLID.Scheduler.Common.DataModels;
using COLID.Scheduler.Services.Interfaces;
using COLID.SchedulerService.Jobs.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace COLID.SchedulerService.Jobs.Implementation
{
    public class StoredQueriesExecutionJob : IStoredQueriesExecutionJob
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ILogger<StoredQueriesExecutionJob> _logger;
        private readonly IRemoteAppDataService _appDataService;

        public StoredQueriesExecutionJob(IBackgroundJobClient backgroundJobClient, ILogger<StoredQueriesExecutionJob> logger, IRemoteAppDataService appDataService)
        {
            _backgroundJobClient = backgroundJobClient;
            _logger = logger;
            _appDataService = appDataService;
        }

        [Queue(Queue.Beta)]
        public async Task ExecuteAsync(CancellationToken token)
        {
            _backgroundJobClient.Enqueue<IStoredQueriesExecutionJob>(x => x.NotifyAllUsersForNewStoredQueryUpdates());
            _logger.LogInformation("StoredQueriesExecution Job Finished");
        }

        public void NotifyAllUsersForNewStoredQueryUpdates()
        {
            _appDataService.ProcessStoredQueries();
            _logger.LogInformation($"ProcessStoredQueries Method in RemoteAppDataService called");
        }
    }
}
