using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using COLID.Scheduler.Common.DataModels;

namespace COLID.Scheduler.Services.Interface
{
    /// <summary>
    /// Service to handle communication and authentication with the external AppDataService
    /// </summary>
    public interface IRemoteAppDataService
    {
        /// <summary>
        /// Get a List of users from external AppDataService
        /// </summary>
        void GetUsers();

        /// <summary>
        /// Get a list of messages to send via email to individual users
        /// </summary>
        /// <returns></returns>
        Task<IList<Message>> GetMessagesToSend();

        /// <summary>
        /// Update the status of the message in the database as sent, after email with the message has been sent to the user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="messageId"></param>
        void MarkMessageAsSent(string userId, int messageId);

        /// <summary>
        /// Checks the validity of multiple users by a query to the Microsoft Graph API and returns them accordingly in the form of an AdUserDto.
        /// In the first step a batch request is created based on the given email addresses. Then all requests are evaluated and only responses
        /// with the status OK are mapped accordingly.All other result types are ignored and mapped to invalid.
        ///
        /// Invalid users will be cached for the configured expiration time and not requested from ms graph.
        /// </summary>
        /// <param name="emailList">user email-adresses to check</param>
        /// <exception cref="ArgumentException">if the email pattern doesn't match</exception>
        Task<IEnumerable<AdUserDto>> CheckUsersValidityAsync(IEnumerable<string> emailList);

        /// <summary>
        /// Triggers the message creation for invalid users to the given dto.
        /// </summary>
        /// <param name="ceciu">the ColidEntryContactInvalidUsersDto to consider</param>
        Task CreateMessagesOfInvalidUsersForContact(ColidEntryContactInvalidUsersDto ceciu);
        
        /// <summary>
        /// Triggers the deletion of expired messages in the appDataService.
        /// </summary>
        void DeleteExpiredMessages();

        /// <summary>
        /// Processes all users stored queries in the appDataService.
        /// </summary>
        void ProcessStoredQueries();

        /// <summary>
        /// Get List of All Save Search Filters from Data Marketplace
        /// </summary>
        /// <returns></returns>
        Task<Dictionary<string, int>> GetAllSubscribedSearchFiltersCountDMP();

        /// <summary>
        /// Get List of All Favorites
        /// </summary>
        /// <returns></returns>
        Task<Dictionary<string, int>> GetAllFavoritesListCount();

        /// <summary>
        /// Get List of All Subscriptions
        /// </summary>
        /// <returns></returns>
        Task<Dictionary<string, int>> GetAllSubscriptionsCount();
    }
}
