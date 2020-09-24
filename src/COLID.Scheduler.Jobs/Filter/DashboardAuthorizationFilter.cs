using System;
using System.Collections.Generic;
using System.Text;
using Hangfire.Dashboard;

namespace COLID.Scheduler.Jobs.Filter
{
    internal class DashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            // Currently everybody is allowed to access the dashboard.
            return true;
        }
    }
}
