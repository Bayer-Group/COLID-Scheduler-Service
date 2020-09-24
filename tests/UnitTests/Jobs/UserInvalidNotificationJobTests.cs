using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COLID.Common.Extensions;
using COLID.Scheduler.Common.DataModels;
using COLID.Scheduler.Services.Interface;
using COLID.SchedulerService.Jobs.Implementation;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Moq;
using UnitTests;
using Xunit;

namespace COLID.SchedulerService.UnitTests.Jobs
{
    public class UserInvalidNotificationJobTests
    {
        private readonly Mock<IRemoteAppDataService> _mockAppDataService = new Mock<IRemoteAppDataService>();
        private readonly Mock<IRemoteReportingService> _mockReportingService = new Mock<IRemoteReportingService>();
        private readonly Mock<IBackgroundJobClient> _mockBackgroundJobClient = new Mock<IBackgroundJobClient>();
        private readonly Mock<ILogger<UserInvalidNotificationJob>> _mockLogger = new Mock<ILogger<UserInvalidNotificationJob>>();

        private readonly UserInvalidNotificationJob _job;

        public UserInvalidNotificationJobTests()
        {
            var dataSteward = TestData.GenerateContactDataSteward("tim.odenthal.ext@bayer.com");
            var lastChangeUser = TestData.GenerateContactLastChangeUser("christian.kaubisch.ext@bayer.com");
            var author = TestData.GenerateContactAuthor("simon.lansing.ext@bayer.com");
            var consumerGroupAdmin = TestData.GenerateContactConsumerGroupAdmin();
            var validAdUserList = new List<AdUserDto>(4)
            {
                new AdUserDto(Guid.NewGuid().ToString(), dataSteward.EmailAddress, true),
                new AdUserDto(Guid.NewGuid().ToString(), lastChangeUser.EmailAddress, true),
                new AdUserDto(Guid.NewGuid().ToString(), author.EmailAddress, true),
                new AdUserDto(Guid.NewGuid().ToString(), consumerGroupAdmin.EmailAddress, true),
            };

            var invalidDataSteward = TestData.GenerateContactDataSteward("invalid@user.com");
            var invalidLastChangeUser = TestData.GenerateContactLastChangeUser("invalid@user.com");
            var invalidAuthor = TestData.GenerateContactAuthor("invalid@user.com");
            var invalidLastChangeUserTwo = TestData.GenerateContactLastChangeUser("invalid2@user.com");
            var invalidAuthorTwo = TestData.GenerateContactAuthor("invalid2@user.com");
            var invalidAuthorThree = TestData.GenerateContactAuthor("invalid3@user.com");
            var invalidAdUserList = new List<AdUserDto>(6)
            {
                new AdUserDto(Guid.NewGuid().ToString(), invalidDataSteward.EmailAddress, false),
                new AdUserDto(Guid.NewGuid().ToString(), invalidLastChangeUser.EmailAddress, false),
                new AdUserDto(Guid.NewGuid().ToString(), invalidAuthor.EmailAddress, false),
                new AdUserDto(Guid.NewGuid().ToString(), invalidLastChangeUserTwo.EmailAddress, false),
                new AdUserDto(Guid.NewGuid().ToString(), invalidAuthorTwo.EmailAddress, false),
                new AdUserDto(Guid.NewGuid().ToString(), invalidAuthorThree.EmailAddress, false),
            };

            var userValidityList = new List<AdUserDto>();
            userValidityList.AddRange(validAdUserList);
            userValidityList.AddRange(invalidAdUserList);

            var contactsFromAllColidEntriesList = userValidityList.Select(x => x.Mail).ToList();
            _mockReportingService.Setup(x => x.GetContactsFromAllColidEntries())
                .ReturnsAsync(contactsFromAllColidEntriesList);

            _mockAppDataService.Setup(x => x.CheckUsersValidityAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(userValidityList);

            var allUsersValidEntry = TestData.GenerateColidEntryContacts("All users valid entry", new List<ContactDto>() { dataSteward, lastChangeUser, author });
            var dataStewardInvalidEntry = TestData.GenerateColidEntryContacts("Data steward invalid entry", new List<ContactDto>() { invalidDataSteward, lastChangeUser, author });
            var lastChangeUserInvalidEntry = TestData.GenerateColidEntryContacts("Last change user invalid entry", new List<ContactDto>() { dataSteward, invalidLastChangeUser, author });
            var authorInvalidEntry = TestData.GenerateColidEntryContacts("Author invalid entry", new List<ContactDto>() { dataSteward, lastChangeUser, invalidAuthor });
            var onlyDataStewardValidEntry = TestData.GenerateColidEntryContacts("only data steward valid entry", new List<ContactDto>() { dataSteward, invalidLastChangeUser, invalidAuthorTwo });
            var onlyLastChangeUserValidEntry = TestData.GenerateColidEntryContacts("only last change user valid entry", new List<ContactDto>() { invalidDataSteward, lastChangeUser, invalidAuthorTwo });
            var onlyAuthorValidEntry = TestData.GenerateColidEntryContacts("only author valid entry", new List<ContactDto>() { invalidDataSteward, invalidLastChangeUserTwo, author });
            var allSameUsersInvalidEntry = TestData.GenerateColidEntryContacts("All same users invalid entry", new List<ContactDto>() { invalidDataSteward, invalidAuthor, invalidLastChangeUser });
            var allDifferentUsersInvalidEntry = TestData.GenerateColidEntryContacts("All different users invalid entry", new List<ContactDto>() { invalidDataSteward, invalidAuthorThree, invalidLastChangeUserTwo });

            var oneValidOneInvalidDataSteward = TestData.GenerateColidEntryContacts("One valid and one invalid Data Steward", new List<ContactDto>() { dataSteward, invalidDataSteward, author, lastChangeUser });

            var colidEntryContactList = new List<ColidEntryContactsDto>()
            {
                allUsersValidEntry,
                dataStewardInvalidEntry,
                lastChangeUserInvalidEntry,
                authorInvalidEntry,
                onlyDataStewardValidEntry,
                onlyLastChangeUserValidEntry,
                onlyAuthorValidEntry,
                allSameUsersInvalidEntry,
                allDifferentUsersInvalidEntry,

                oneValidOneInvalidDataSteward
            };

            _mockReportingService.Setup(x => x.GetContactReferencedEntries(It.IsAny<string>()))
                .ReturnsAsync((string isAnyParam) => colidEntryContactList.Where(e => e.Contacts.Any(c => c.EmailAddress == isAnyParam)));

            _mockBackgroundJobClient.Setup(x => x.Create(It.IsAny<Job>(), It.IsAny<IState>()));

            _job = new UserInvalidNotificationJob(_mockBackgroundJobClient.Object, _mockAppDataService.Object, _mockReportingService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CollectAndPrepareContactMessagesForEntries_Should_Call_Endpoints()
        {
            var resultList = (await _job.CollectAndPrepareContactMessagesForEntries()).ToList();

            Assert.NotNull(resultList);
            Assert.NotEmpty(resultList);

            var christian = resultList.First(x => x.ContactMail.Equals("christian.kaubisch.ext@bayer.com"));
            Assert.NotNull(christian);
            Assert.Equal(2, christian.ColidEntries.Count);

            var simon = resultList.First(x => x.ContactMail.Equals("simon.lansing.ext@bayer.com"));
            Assert.NotNull(simon);
            Assert.Equal(1, simon.ColidEntries.Count);

            var tim = resultList.FirstOrDefault(x => x.ContactMail.Equals("tim.odenthal.ext@bayer.com"));
            Assert.NotNull(tim);
            Assert.Equal(1, tim.ColidEntries.Count);

            var cgAdmin = resultList.First(x => x.ContactMail.Equals("consumer.group.admin@bayer.com"));
            Assert.NotNull(cgAdmin);
            Assert.Equal(2, cgAdmin.ColidEntries.Count);
        }


        /// <summary>
        /// This test is for the case, that a resource has a valid user in author, lastchangeuser and datasteward and
        /// one invalid user added as a data steward. Therefor only one message should be created
        /// </summary>
        [Fact]
        public async Task CollectAndPrepareContactMessagesForEntries_Should_Call_Endpoints_Once()
        {
            // ARRANGE
            var user = "tim.odenthal.ext@bayer.com";
            var invalidUser = "dinos@bayer.com";
            var colidEntryLabel = "One valid and one invalid Data Steward";

            var dataSteward = TestData.GenerateContactDataSteward(user);
            var lastChangeUser = TestData.GenerateContactLastChangeUser(user);
            var author = TestData.GenerateContactAuthor(user);
            var validAdUserList = new List<AdUserDto>(1) { new AdUserDto(Guid.NewGuid().ToString(), user, true) };
            var invalidDataSteward = TestData.GenerateContactDataSteward(invalidUser);
            var invalidAdUserList = new List<AdUserDto>(1) { new AdUserDto(Guid.NewGuid().ToString(), invalidDataSteward.EmailAddress, false) };
            var userValidityList = new HashSet<AdUserDto>();
            userValidityList.AddRange(validAdUserList);
            userValidityList.AddRange(invalidAdUserList);
            var contactsFromAllColidEntriesList = userValidityList.Select(x => x.Mail).ToList();
            _mockReportingService.Setup(x => x.GetContactsFromAllColidEntries()).ReturnsAsync(contactsFromAllColidEntriesList);
            _mockAppDataService.Setup(x => x.CheckUsersValidityAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(userValidityList);
            var oneValidOneInvalidDataSteward = TestData.GenerateColidEntryContacts(colidEntryLabel, new List<ContactDto> { dataSteward, invalidDataSteward, author, lastChangeUser });
            var colidEntryContactList = new List<ColidEntryContactsDto> { oneValidOneInvalidDataSteward };
            _mockReportingService.Setup(x => x.GetContactReferencedEntries(It.IsAny<string>()))
                .ReturnsAsync((string isAnyParam) => colidEntryContactList.Where(e => e.Contacts.Any(c => c.EmailAddress == isAnyParam)));
            _mockBackgroundJobClient.Setup(x => x.Create(It.IsAny<Job>(), It.IsAny<IState>()));

            // ACT
            var resultList = (await _job.CollectAndPrepareContactMessagesForEntries()).ToList();
           
            // ASSERT
            Assert.NotNull(resultList);
            Assert.Single(resultList);
            var result = resultList.First();
            Assert.Equal(user, result.ContactMail);
            Assert.Single(result.ColidEntries);
            var colidEntries = result.ColidEntries.First();
            Assert.Single(colidEntries.InvalidUsers);
            Assert.Equal(invalidUser, colidEntries.InvalidUsers.First());
            Assert.Equal(colidEntryLabel, colidEntries.Label);
        }

    }
}
