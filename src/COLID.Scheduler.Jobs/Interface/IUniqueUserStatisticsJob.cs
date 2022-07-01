using System;
using System.Collections.Generic;
using System.Text;
using COLID.SchedulerService.Jobs.Interface;

namespace COLID.Scheduler.Jobs.Interface
{
    /// <summary>
    /// This Job fetches and writes unique user counts to Elasticsearch.
    /// </summary>
    public interface IUniqueUserStatisticsJob : IJob
    {
    }
}
