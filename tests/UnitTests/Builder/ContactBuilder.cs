using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using COLID.Scheduler.Common.DataModels;
using Microsoft.Extensions.Configuration;

namespace UnitTests.Builder
{
    public class ContactBuilder
    {
        private ContactDto _contact = new ContactDto();
        private static readonly string _basePath = Path.GetFullPath("appsettings.json");
        private static readonly string _filePath = _basePath.Substring(0, _basePath.Length - 16);
        private static IConfigurationRoot _configuration = new ConfigurationBuilder()
                .SetBasePath(_filePath)
            .AddJsonFile("appsettings.json")
            .Build();
        public static readonly string _serviceUrl = _configuration.GetValue<string>("ServiceUrl");
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
            WithTypeUri(new Uri(_serviceUrl + "kos/19050/hasContactPerson"));
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
