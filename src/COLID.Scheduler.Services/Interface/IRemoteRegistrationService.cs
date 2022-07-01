using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace COLID.Scheduler.Services.Interface
{
    public interface IRemoteRegistrationService
    {
        Task CheckDistributionEndpointValidityAndNotifyUsersAsync();
    }
}
