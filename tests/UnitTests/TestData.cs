using System;
using System.Collections.Generic;
using System.Text;
using COLID.Scheduler.Common.DataModels;
using UnitTests.Builder;

namespace UnitTests
{
    public static class TestData
    {
        public static AdUserDto GenerateAdUser(bool enabled = true)
        {
            var id = Guid.NewGuid().ToString();
            var mail = $"{id.Substring(0, 8)}@bayer.com";
            return new AdUserDto(id, mail, enabled);
        }

        public static ColidEntryContactsDto GenerateColidEntryContacts(string colidEntryLabel, IEnumerable<ContactDto> contacts)
        {
            return new ColidEntryContactBuilder()
                .WithPidUri($"https://pid.bayer.com/{Guid.NewGuid()}")
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
                .WithTypeUri("https://pid.bayer.com/kos/19050/hasConsumerGroupContactPerson")
                .WithTypeLabel("Contact Person")
                .Build();
        }

        public static ContactDto GenerateContactContactPerson(string email)
        {
            return new ContactBuilder()
                .WithEmailAddress(email)
                .WithTypeUri("https://pid.bayer.com/kos/19050/hasContactPerson")
                .WithTypeLabel("Contact Person")
                .IsTechnical(false)
                .Build();
        }

        public static ContactDto GenerateContactLastChangeUser(string email)
        {
            return new ContactBuilder()
                .WithEmailAddress(email)
                .WithTypeUri("https://pid.bayer.com/kos/19050/lastChangeUser")
                .WithTypeLabel("Last Change User")
                .IsTechnical(true)
                .Build();
        }

        public static ContactDto GenerateContactAuthor(string email)
        {
            return new ContactBuilder()
                .WithEmailAddress(email)
                .WithTypeUri("https://pid.bayer.com/kos/19050/author")
                .WithTypeLabel("Author")
                .IsTechnical(true)
                .Build();
        }

        public static ContactDto GenerateContactDataSteward(string email)
        {
            return new ContactBuilder()
                .WithEmailAddress(email)
                .WithTypeUri("https://pid.bayer.com/kos/19050/hasDataSteward")
                .WithTypeLabel("Data Steward")
                .IsTechnical(false)
                .Build();
        }

        #endregion ContactDto
    }
}
