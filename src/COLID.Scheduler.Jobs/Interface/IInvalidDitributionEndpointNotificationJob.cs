using System;
using System.Collections.Generic;
using System.Text;

namespace COLID.SchedulerService.Jobs.Interface
{
    /// <summary>
    /// Job to trigger notification informing user about invalid distribution endpoints associated with their resource(s).
    /// </summary>
    interface IInvalidDitributionEndpointNotificationJob : IJob
    {
        public void NotifyUsersForInvalidDistributionEndpoints();
    }
}
