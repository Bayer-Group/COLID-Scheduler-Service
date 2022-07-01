using System;
using System.Collections.Generic;
using System.Text;
using COLID.SchedulerService.Jobs.Interface;

namespace COLID.Scheduler.Jobs.Interface
{
    /// <summary>
    /// This Job fetches the ConsumerGroup, LifecycleStatus, InformationClassification and ResourceType
    /// statistics from Reporting Service and writes them into Elasticsearch using COLID.StatisticsLog package.
    /// </summary>
    public interface IResourceStatisticsJob : IJob
    {

    }
}
