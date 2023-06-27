using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using COLID.Scheduler.Services.Implementation;
using COLID.Scheduler.Services.Interfaces;
using COLID.Scheduler.Services.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace UnitTests.services
{
    public class MailServiceTests
    {
        private readonly Mock<ILogger<MailService>> _mockLogger = new Mock<ILogger<MailService>>();
        private readonly Mock<IOptionsSnapshot<SmtpOptions>> _mockSmtpOptions = new Mock<IOptionsSnapshot<SmtpOptions>>();
        private readonly Mock<SmtpClient> _mockSmtpClient = new Mock<SmtpClient>();

        private readonly IMailService _mail;

        public MailServiceTests()
        {
            var smtpOptions = new SmtpOptions()
            {
                Server = "blia@bla.com",
                Port = 25,
                EnableSsl = false,
                User = "batman@gothamcity.com",
                Password = "wow, much secure",
                Sender = "colid-dev@notification.dev.daaa.cloud"
            };
            _mockSmtpOptions.Setup(x => x.Value).Returns(smtpOptions);

            _mail = new MailService(_mockSmtpOptions.Object, _mockLogger.Object);
        }

        public void TestValidEmail()
        {
            // ARRANGE
            const string subject = "This is a very handsome subject";
            const string body = "<h1>Wow, such body!</h1>";
            const string to = "christian.kaubisch.ext@bayer.com";
            MailMessage interceptedMessage = null;
            _mockSmtpClient.Setup(client => client.Send(It.IsAny<MailMessage>()))
                .Callback<MailMessage>(msg => interceptedMessage = msg);

            // ACT
            var success = _mail.SendEmail(subject, body, to);

            // ASSERT
            Assert.NotNull(interceptedMessage); // callback-verification
            Assert.True(success);
            Assert.Equal(subject, interceptedMessage.Subject);
            Assert.Equal(body, interceptedMessage.Body);
            Assert.Equal(to, interceptedMessage.To.First().ToString());
            Assert.Equal("colid-power@notification.abc.def.ghi", interceptedMessage.From.ToString());
            Assert.True(interceptedMessage.IsBodyHtml);
        }
    }
}
