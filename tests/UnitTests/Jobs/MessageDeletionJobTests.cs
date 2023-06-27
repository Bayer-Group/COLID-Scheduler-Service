using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COLID.Common.Extensions;
using COLID.Scheduler.Common.DataModels;
using COLID.Scheduler.Services.Interfaces;
using COLID.SchedulerService.Jobs.Implementation;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Moq;
using UnitTests;
using Xunit;

namespace COLID.SchedulerService.UnitTests.Jobs
{
    public class MessageDeletionJobTests
    {
        private readonly Mock<IRemoteAppDataService> _mockAppDataService = new Mock<IRemoteAppDataService>();
        private readonly Mock<IBackgroundJobClient> _mockBackgroundJobClient = new Mock<IBackgroundJobClient>();
        private readonly Mock<ILogger<MessageDeletionJob>> _mockLogger = new Mock<ILogger<MessageDeletionJob>>();
        private readonly MessageDeletionJob _job;

        public MessageDeletionJobTests()
        {
            _mockBackgroundJobClient.Setup(x => x.Create(It.IsAny<Job>(), It.IsAny<IState>()));
            _job = new MessageDeletionJob(_mockBackgroundJobClient.Object, _mockLogger.Object,_mockAppDataService.Object);
        }

        /// <summary>
        /// This test ensures that MessageDeletionJob calls the DeleteExpiredMessage Method in the RemoteAppDataService 
        /// Thus the Delete Method in the AppDataService will be called
        /// </summary> 
        public async Task DeleteExpiredMessages_should_call_endpoints()
        {
            _job.DeleteExpiredMessages();
            _mockAppDataService.Verify(x => x.DeleteExpiredMessages(), Times.Once);
        }
    }
}
