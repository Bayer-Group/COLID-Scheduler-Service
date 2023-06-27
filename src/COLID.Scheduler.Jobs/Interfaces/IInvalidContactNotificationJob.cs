using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using COLID.SchedulerService.Jobs.Interfaces;

namespace COLID.Scheduler.Jobs.Interfaces
{
    public interface IInvalidContactNotificationJob : IJob
    {
        Task CheckDataStewardsAndDistributionEndpointContactsAndNotifyUsers();
    }
}
