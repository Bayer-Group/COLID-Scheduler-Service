using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using COLID.Scheduler.Common;
using COLID.Scheduler.Common.Constants;
using COLID.Scheduler.Common.DataModels;
using COLID.Scheduler.Jobs.Interfaces;
using COLID.Scheduler.Services.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;
using Queue = COLID.Scheduler.Common.Constants.Queue;

namespace COLID.SchedulerService.Jobs.Implementation
{
    public class UserInvalidNotificationJob : IUserInvalidNotificationJob
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IRemoteAppDataService _appDataService;
        private readonly IRemoteReportingService _reportingService;
        private readonly ILogger<UserInvalidNotificationJob> _logger;

        public UserInvalidNotificationJob(IBackgroundJobClient backgroundJobClient, IRemoteAppDataService appDataService, IRemoteReportingService reportingService, ILogger<UserInvalidNotificationJob> logger)
        {
            _backgroundJobClient = backgroundJobClient;
            _appDataService = appDataService;
            _reportingService = reportingService;
            _logger = logger;
        }

        [Queue(Queue.Beta)]
        public async Task ExecuteAsync(CancellationToken token)
        {
            var contactAndEntryList = await CollectAndPrepareContactMessagesForEntries();

            _logger.LogInformation($"Starting UserInvalidNotificationJob");
            foreach (var contact in contactAndEntryList)
            {
                _logger.LogInformation($"Processing user {contact.ContactMail} ...");
                _backgroundJobClient.Enqueue(() => _appDataService.CreateMessagesOfInvalidUsersForContact(contact));
            }
            _logger.LogInformation("Finished");
        }

        public async Task<IEnumerable<ColidEntryContactInvalidUsersDto>> CollectAndPrepareContactMessagesForEntries()
        {
            var contactAndEntryList = new List<ColidEntryContactInvalidUsersDto>();
            var adUserEmailSet = await _reportingService.GetContactsFromAllColidEntries();
            var adUserValidEmailSet = RemoveInvalidEmailsByPattern(adUserEmailSet);
            var contactValidityList = await _appDataService.CheckUsersValidityAsync(adUserValidEmailSet);

            // ad invalid emails by pattern to invalid contacts -> allow any string in reporting service
            var invalidContacts = contactValidityList.Where(x => !x.AccountEnabled);
            var invalidContactsMails = invalidContacts.Select(c => c.Mail).ToHashSet();

            foreach (var adUserDto in invalidContacts)
            {
                var entityList = await _reportingService.GetContactReferencedEntries(adUserDto.Mail);

                foreach (var colidEntryContactsDto in entityList)
                {
                    if (IsTechnicalUserInEntry(colidEntryContactsDto, adUserDto.Mail))
                    {
                        continue; // then skip message creation
                    }

                    var contactPerson = DetermineContactPerson(adUserDto.Mail, colidEntryContactsDto, invalidContactsMails);
                    var contactAndEntries = contactAndEntryList.FirstOrDefault(c => c.ContactMail == contactPerson);

                    if (contactAndEntries != null)
                    {
                        if (contactAndEntries.TryGetColidEntry(colidEntryContactsDto.PidUri, out ColidEntryInvalidUsersDto colidEntry))
                        {
                            // append invalid user to existing users' colid entry
                            colidEntry.InvalidUsers.Add(adUserDto.Mail);
                        }
                        else
                        {
                            // create new colid entry for user
                            var ce = new ColidEntryInvalidUsersDto(colidEntryContactsDto.PidUri,
                                colidEntryContactsDto.Label, new HashSet<string> { adUserDto.Mail });
                            contactAndEntries.ColidEntries.Add(ce);
                        }
                    }
                    else
                    {
                        // create new contact message
                        var invalidUserDto = new ColidEntryInvalidUsersDto(colidEntryContactsDto.PidUri,
                            colidEntryContactsDto.Label, new HashSet<string> { adUserDto.Mail });
                        var contactDto = new ColidEntryContactInvalidUsersDto();
                        contactDto.ContactMail = contactPerson;
                        contactDto.ColidEntries.Add(invalidUserDto);
                        contactAndEntryList.Add(contactDto);
                    }
                }
            }

            return contactAndEntryList;
        }

        private static bool IsTechnicalUserInEntry(ColidEntryContactsDto colidEntryContactsDto, string adUserMail)
        {
            return colidEntryContactsDto.Contacts
                .Where(c => c.EmailAddress == adUserMail)
                .All(c => c.IsTechnicalContact);
        }

        private static IEnumerable<string> RemoveInvalidEmailsByPattern(IEnumerable<string> adUserEmailSet)
        {
            var validEmails = new HashSet<string>(adUserEmailSet.Count());
            foreach (var potentialMail in adUserEmailSet)
            {
                if (RegexUtilities.IsValidEmail(potentialMail))
                {
                    validEmails.Add(potentialMail);
                }
            }

            return validEmails;
        }

        /// <summary>
        /// Determine the person to contact of invalid users. E.g. if a registered entry in COLID has 4 different users, the contact person will
        /// be determined by the following hierarchy (but only if the user itself is valid):
        ///   1. Data Steward
        ///   2. Last Change User
        ///   3. Author
        ///   4. Consumer Group Admin
        /// The consumer group admin will only be used, if all others are no longer valid users.
        /// </summary>
        private static string DetermineContactPerson(string emailAddressToCheck, ColidEntryContactsDto colidEntryContactsDto, HashSet<string> invalidContactsMails)
        {
            string contactPerson = string.Empty;
            if (colidEntryContactsDto.Contacts.All(contact => invalidContactsMails.Contains(contact.EmailAddress)))
            {
                return colidEntryContactsDto.ConsumerGroupContact.EmailAddress;
            }

            var validContactsFromEntry = colidEntryContactsDto.Contacts.Where(contact => !invalidContactsMails.Contains(contact.EmailAddress)).ToList();

            var dataSteward = validContactsFromEntry.FirstOrDefault(c => c.TypeUri.OriginalString == Metadata.DataSteward);
            if (dataSteward != null)
            {
                contactPerson = dataSteward.EmailAddress;
            }

            var lastChangeUser = validContactsFromEntry
                .FirstOrDefault(c => c.TypeUri.OriginalString == Metadata.LastChangeUser);
            if (lastChangeUser != null && string.IsNullOrEmpty(contactPerson))
            {
                contactPerson = lastChangeUser.EmailAddress;
            }

            var author = validContactsFromEntry.FirstOrDefault(c => c.TypeUri.OriginalString == Metadata.Author);
            if (author != null && string.IsNullOrEmpty(contactPerson))
            {
                contactPerson = author.EmailAddress;
            }

            return contactPerson;
        }
    }
}
