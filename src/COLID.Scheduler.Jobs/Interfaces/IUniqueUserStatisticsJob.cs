using System;
using System.Collections.Generic;
using System.Text;
using COLID.SchedulerService.Jobs.Interfaces;

namespace COLID.Scheduler.Jobs.Interfaces
{
    /// <summary>
    /// This Job fetches and writes unique user counts to Elasticsearch.
    /// </summary>
    public interface IUniqueUserStatisticsJob : IJob
    {
    }
}
