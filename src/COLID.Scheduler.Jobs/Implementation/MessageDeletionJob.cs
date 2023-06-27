using System;
using System.Threading;
using System.Threading.Tasks;
using COLID.Scheduler.Common.Constants;
using COLID.Scheduler.Services.Interfaces;
using COLID.SchedulerService.Jobs.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace COLID.SchedulerService.Jobs.Implementation
{
    public class MessageDeletionJob : IMessageDeletionJob
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ILogger<MessageDeletionJob> _logger;
        private readonly IRemoteAppDataService _appDataService;

        public MessageDeletionJob(IBackgroundJobClient backgroundJobClient, ILogger<MessageDeletionJob> logger, IRemoteAppDataService appDataService)
        {
            _backgroundJobClient = backgroundJobClient;
            _logger = logger;
            _appDataService = appDataService;
        }

        [Queue(Queue.Beta)]
        public async Task ExecuteAsync(CancellationToken token)
        {
            _backgroundJobClient.Enqueue<IMessageDeletionJob>(x => x.DeleteExpiredMessages());
            _logger.LogInformation("Message Deletion Job Finished");
        }

        public void DeleteExpiredMessages()
        {
            _appDataService.DeleteExpiredMessages();
            _logger.LogInformation($"DeleteExpiredMessages Method in RemoteAppDataService called");
        }
    }
}
