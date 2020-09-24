using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COLID.Scheduler.Common.DataModels
{
    public class ColidEntryInvalidUsersDto
    {
        public Uri PidUri { get; set; }

        public string Label { get; set; }

        public ISet<string> InvalidUsers { get; set; }

        public ColidEntryInvalidUsersDto(Uri pidUri, string label, ISet<string> invalidUsers)
        {
            PidUri = pidUri;
            Label = label;
            InvalidUsers = invalidUsers;
        }
    }
}
