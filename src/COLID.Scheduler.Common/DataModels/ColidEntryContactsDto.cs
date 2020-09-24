using System;
using System.Collections.Generic;

namespace COLID.Scheduler.Common.DataModels
{
    public class ColidEntryContactsDto
    {
        /// <summary>
        /// PidUri of colid entry
        /// </summary>
        public Uri PidUri { get; set; }

        /// <summary>
        /// Label of colid entry
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// List of all contacts with her roles in the entry
        /// </summary>
        public IEnumerable<ContactDto> Contacts { get; set; }

        /// <summary>
        /// Contact of the consumer group assigned to the entry
        /// </summary>
        public ContactDto ConsumerGroupContact { get; set; }

        public ColidEntryContactsDto()
        {
            Contacts ??= new List<ContactDto>();
        }

        public ColidEntryContactsDto(Uri pidUri, string label, IEnumerable<ContactDto> contacts, ContactDto consumerGroupContact)
        {
            PidUri = pidUri;
            Label = label;
            Contacts = contacts;
            ConsumerGroupContact = consumerGroupContact;
        }
    }
}
