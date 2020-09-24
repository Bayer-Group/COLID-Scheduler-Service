using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using COLID.Scheduler.Common.DataModels;

namespace COLID.Scheduler.Services.Interface
{
    /// <summary>
    /// Service to handle communication and authentication with the external ReportingService.
    /// </summary>
    public interface IRemoteReportingService
    {
        /// <summary>
        /// Returns a list containing all contacts referenced in the database
        /// </summary>
        /// <returns>A list of contacts ids</returns>
        Task<IEnumerable<string>> GetContactsFromAllColidEntries();

        /// <summary>
        /// Returns a list of entries in which the user is referenced.
        /// </summary>
        /// <param name="userEmailAddress">The email address of the user to search for.</param>
        /// <returns>List of all entries with their contact contacts.</returns>
        Task<IEnumerable<ColidEntryContactsDto>> GetContactReferencedEntries(string userEmailAddress);
    }
}
