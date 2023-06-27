using System;
using System.Threading;
using System.Threading.Tasks;
using COLID.Scheduler.Common.Constants;
using COLID.SchedulerService.Jobs.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace COLID.SchedulerService.Jobs.Implementation
{
    public class EntryChangedNotificationJob : IEntryChangedNotificationJob
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ILogger<EntryChangedNotificationJob> _logger;

        public EntryChangedNotificationJob(IBackgroundJobClient backgroundJobClient, ILogger<EntryChangedNotificationJob> logger)
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
