using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using COLID.Scheduler.Common.DataModels;
using Microsoft.Extensions.Configuration;
using UnitTests.Builder;
using COLID.Scheduler.Common.Constants;

namespace UnitTests
{
    public static class TestData
    {
        private static readonly string _basePath = Path.GetFullPath("appsettings.json");
        private static readonly string _filePath = _basePath.Substring(0, _basePath.Length - 16);
        private static IConfigurationRoot _configuration = new ConfigurationBuilder()
                .SetBasePath(_filePath)
            .AddJsonFile("appsettings.json")
            .Build();
        public static readonly string _serviceUrl = _configuration.GetValue<string>("ServiceUrl");
        public static AdUserDto GenerateAdUser(bool enabled = true)
        {
            var id = Guid.NewGuid().ToString();
            var mail = $"{id.Substring(0, 8)}@bayer.com";
            return new AdUserDto(id, mail, enabled);
        }

        public static ColidEntryContactsDto GenerateColidEntryContacts(string colidEntryLabel, IEnumerable<ContactDto> contacts)
        {
            return new ColidEntryContactBuilder()
                .WithPidUri($"{_serviceUrl}{Guid.NewGuid()}")
                .WithLabel(colidEntryLabel)
                .WithContacts(contacts)
                .WithConsumerGroupContact(GenerateContactConsumerGroupAdmin())
                .Build();
        }

        #region AdUserDto

        public static AdUserDto GenerateAdUserActive(string email)
        {
            return new AdUserDto(Guid.NewGuid().ToString(), email, true);
        }

        public static AdUserDto GenerateAdUserInActive(string email)
        {
            return new AdUserDto(Guid.NewGuid().ToString(), email, false);
        }

        #endregion AdUserDto

        #region ContactDto

        public static ContactDto GenerateContactConsumerGroupAdmin()
        {
            return new ContactBuilder()
                .WithEmailAddress("consumer.group.admin@bayer.com")
                .WithTypeUri(Metadata.ConsumerGroupContact)
                .WithTypeLabel("Contact Person")
                .Build();
        }

        public static ContactDto GenerateContactContactPerson(string email)
        {
            return new ContactBuilder()
                .WithEmailAddress(email)
                .WithTypeUri(Metadata.ContactPerson)
                .WithTypeLabel("Contact Person")
                .IsTechnical(false)
                .Build();
        }

        public static ContactDto GenerateContactLastChangeUser(string email)
        {
            return new ContactBuilder()
                .WithEmailAddress(email)
                .WithTypeUri(Metadata.LastChangeUser)
                .WithTypeLabel("Last Change User")
                .IsTechnical(true)
                .Build();
        }

        public static ContactDto GenerateContactAuthor(string email)
        {
            return new ContactBuilder()
                .WithEmailAddress(email)
                .WithTypeUri(Metadata.Author)
                .WithTypeLabel("Author")
                .IsTechnical(true)
                .Build();
        }

        public static ContactDto GenerateContactDataSteward(string email)
        {
            return new ContactBuilder()
                .WithEmailAddress(email)
                .WithTypeUri(Metadata.DataSteward)
                .WithTypeLabel("Data Steward")
                .IsTechnical(false)
                .Build();
        }

        #endregion ContactDto
    }
}
