using System;
using System.Collections.Generic;
using System.Text;

namespace COLID.Scheduler.Common.DataModels
{
    public class AdUserDto
    {
        public string Id { get; set; }

        public string Mail { get; set; }

        public bool AccountEnabled { get; set; }

        public AdUserDto(string id, string mail, bool accEnabled)
        {
            Id = id;
            Mail = mail;
            AccountEnabled = accEnabled;
        }
    }
}
