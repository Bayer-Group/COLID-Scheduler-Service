using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using COLID.Scheduler.Common.DataModels;
using Nest;
using Newtonsoft.Json.Linq;

namespace COLID.Scheduler.Services.Interface
{
    /// <summary>
    /// Service to handle communication and authentication with the external SearchService.
    /// </summary>
    public interface IRemoteSearchService
    {
        /// <summary>
        /// Searches the Elastic Search client for Unique users in PID and DMP
        /// via search service and writes the unique user document in the index
        /// </summary>
        /// <param name="appId">Application appID enum for which users need to be indexed</param>
        Task WriteUsersToIndex(int appId);
    }
}
