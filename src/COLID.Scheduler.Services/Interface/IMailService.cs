using System;
using System.Collections.Generic;
using System.Text;

namespace COLID.Scheduler.Services.Interface
{
    /// <summary>
    /// Service to send mails via the AWS SES
    /// </summary>
    public interface IMailService
    {
        bool SendEmail(string subject, string body, string recipient);
    }
}
