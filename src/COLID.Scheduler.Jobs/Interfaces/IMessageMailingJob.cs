using System.Collections.Generic;
using COLID.Scheduler.Common.DataModels;

namespace COLID.SchedulerService.Jobs.Interfaces
{
    /// <summary>
    /// Job to distribute the mailing of (stored) messages to the user.
    /// </summary>
    public interface IMessageMailingJob : IJob
    {
        /// <summary>
        /// Combine all messages for a user into one and sends it via email to the specified email address. After successful sending of the email
        /// all messages which will be marked as sent in the database.
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="messages"></param>
        public void SendMessageForUser(string emailAddress, ICollection<Message> messages);

        /// <summary>
        /// Sets all messages of a user to the status sent.
        /// </summary>
        /// <param name="messages"></param>
        public void MarkMessagesAsSent(ICollection<Message> messages);
    }
}
