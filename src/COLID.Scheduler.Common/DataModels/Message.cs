using System;
using System.Collections.Generic;
using System.Text;

namespace COLID.Scheduler.Common.DataModels
{
    public class Message
    {
        public int Id { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        // When the message has been raed
        public DateTime? ReadOn { get; set; }

        // When should the message be send to the user
        public DateTime? SendOn { get; set; }

        // When the message should be deleted
        public DateTime? DeleteOn { get; set; }

        public string UserId { get; set; }

        public string UserEmail { get; set; }
    }
}
