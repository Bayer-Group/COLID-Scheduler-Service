using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COLID.Scheduler.Common.DataModels;

namespace UnitTests.Builder
{
    public class ColidEntryContactBuilder
    {
        private ColidEntryContactsDto _cec = new ColidEntryContactsDto();

        public ColidEntryContactsDto Build()
        {
            return _cec;
        }

        public ColidEntryContactBuilder WithPidUri(Uri pidUri)
        {
            _cec.PidUri = pidUri;
            return this;
        }

        public ColidEntryContactBuilder WithPidUri(string pidUri)
        {
            _cec.PidUri = new Uri(pidUri);
            return this;
        }

        public ColidEntryContactBuilder WithLabel(string label)
        {
            _cec.Label = label;
            return this;
        }

        public ColidEntryContactBuilder WithConsumerGroupContact(ContactDto contact)
        {
            _cec.ConsumerGroupContact = contact;
            return this;
        }

        public ColidEntryContactBuilder WithContacts(IEnumerable<ContactDto> contacts)
        {
            _cec.Contacts = contacts;
            return this;
        }

        public ColidEntryContactBuilder AddContact(ContactDto contact)
        {
            _cec.Contacts.Append(contact);
            return this;
        }
    }
}
