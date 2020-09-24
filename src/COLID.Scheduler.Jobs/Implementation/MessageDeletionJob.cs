using System;
using System.Threading;
using System.Threading.Tasks;
using COLID.Scheduler.Common.Constants;
using COLID.SchedulerService.Jobs.Interface;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace COLID.SchedulerService.Jobs.Implementation
{
    public class MessageDeletionJob : IMessageDeletionJob
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ILogger<MessageDeletionJob> _logger;

        public MessageDeletionJob(IBackgroundJobClient backgroundJobClient, ILogger<MessageDeletionJob> logger)
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
