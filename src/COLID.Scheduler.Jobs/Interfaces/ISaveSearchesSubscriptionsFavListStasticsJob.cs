using System;
using System.Collections.Generic;
using System.Text;
using COLID.SchedulerService.Jobs.Interfaces;

namespace COLID.Scheduler.Jobs.Interfaces
{
    /// <summary>
    /// This Job fetches the Saved Searches, Subscriptions and Favorites List 
    /// statistics from AppData Servicce and writes them into Elasticsearch using COLID.StatisticsLog package.
    /// </summary>
    public interface ISaveSearchesSubscriptionsFavListStasticsJob : IJob
    {

    }
}
