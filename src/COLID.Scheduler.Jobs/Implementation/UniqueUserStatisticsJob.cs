using System;
using System.Threading;
using System.Threading.Tasks;
using COLID.Scheduler.Common.Constants;
using COLID.Scheduler.Jobs.Interface;
using COLID.SchedulerService.Jobs.Interface;
using Hangfire;
using Microsoft.Extensions.Logging;
using COLID.StatisticsLog.Services;
using COLID.Scheduler.Services.Interface;
using COLID.Scheduler.Common.DataModels;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace COLID.SchedulerService.Jobs.Implementation
{
    public class UniqueUserStatisticsJob : IUniqueUserStatisticsJob
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ILogger<UniqueUserStatisticsJob> _logger;
        private readonly IRemoteSearchService _remoteSearchService;


        public UniqueUserStatisticsJob(IBackgroundJobClient backgroundJobClient, ILogger<UniqueUserStatisticsJob> logger, 
            IRemoteSearchService remoteSearchService
            )
        {
            _backgroundJobClient = backgroundJobClient;
            _logger = logger;
            _remoteSearchService = remoteSearchService;
        }

        public async Task ExecuteAsync(CancellationToken token)
        {
            _logger.LogInformation("Hangfire Job begins for Unique User statistics");

            await _remoteSearchService.WriteUsersToIndex(0);
            _logger.LogInformation("Unique User Creation for PID finished");

            await _remoteSearchService.WriteUsersToIndex(1);
            _logger.LogInformation("Unique User Creation for Data Marketplace finished");
        }
    }
}
