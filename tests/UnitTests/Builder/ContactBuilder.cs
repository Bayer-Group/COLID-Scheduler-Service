using System;
using System.Collections.Generic;
using System.Text;
using COLID.Scheduler.Common.DataModels;

namespace UnitTests.Builder
{
    public class ContactBuilder
    {
        private ContactDto _contact = new ContactDto();

        public ContactBuilder()
        {
        }

        public ContactDto Build()
        {
            return _contact;
        }

        public ContactBuilder GenerateSampleData()
        {
            WithEmailAddress("marcus.davies@bayer.com");
            WithTypeUri(new Uri("https://pid.bayer.com/kos/19050/hasContactPerson"));
            WithTypeLabel("Contact Person");
            IsTechnical(false);
            return this;
        }

        public ContactBuilder WithEmailAddress(string email)
        {
            _contact.EmailAddress = email;
            return this;
        }

        public ContactBuilder WithTypeUri(string typeUri)
        {
            _contact.TypeUri = new Uri(typeUri);
            return this;
        }

        public ContactBuilder WithTypeUri(Uri typeUri)
        {
            _contact.TypeUri = typeUri;
            return this;
        }

        public ContactBuilder WithTypeLabel(string typeLabel)
        {
            _contact.TypeLabel = typeLabel;
            return this;
        }
        public ContactBuilder IsTechnical(bool technical)
        {
            _contact.IsTechnicalContact = technical;
            return this;
        }
    }
}
