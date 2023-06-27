using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace COLID.Scheduler.Services.Interfaces
{
    public interface IRemoteRegistrationService
    {
        Task CheckDistributionEndpointValidityAndNotifyUsersAsync();
        Task CheckDataStewardsAndDistributionEndpointContactsAndNotifyUsers();
        Task SetBrokenFlagsInElastic();

        Task<Dictionary<string, string>> NotifyDataStewardsForDueResourceReview();
    }
}
