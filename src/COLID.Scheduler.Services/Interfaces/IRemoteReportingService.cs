using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using COLID.Scheduler.Common.DataModels;

namespace COLID.Scheduler.Services.Interfaces
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

        /// <summary>
        ///  Returns the number of different expressions of used consumer groups
        /// </summary>
        /// <returns>A list of consumer groups</returns>
        Task<IEnumerable<PropertyCharacteristicDto>> GetConsumerGroupCharacteristics();

        /// <summary>
        /// Returns the number of different expressions of used information classification
        /// </summary>
        /// <returns>A list of expression counts</returns>
        Task<IEnumerable<PropertyCharacteristicDto>> GetInformationClassificationCharacteristics();

        /// <summary>
        /// Returns the number of different expressions COLID Types
        /// </summary>
        /// <returns>A list of expression counts for colid types</returns>
        Task<IEnumerable<PropertyCharacteristicDto>> GetResourceTypeCharacteristics();

        /// <summary>
        /// Returns the number of different expressions resource lifecycle statuses
        /// </summary>
        /// <returns>A list of expression counts for colid types</returns>
        Task<IEnumerable<PropertyCharacteristicDto>> GetLifecycleStatusCharacteristics();
    }
}
