using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COLID.Scheduler.Common.DataModels
{
    public class ColidEntryContactInvalidUsersDto
    {
        public string ContactMail { get; set; }

        public IList<ColidEntryInvalidUsersDto> ColidEntries { get; set; }

        public ColidEntryContactInvalidUsersDto()
        {
            ColidEntries = new List<ColidEntryInvalidUsersDto>();
        }

        public bool TryGetColidEntry(Uri pidUri, out ColidEntryInvalidUsersDto colidEntry)
        {
            colidEntry = ColidEntries.FirstOrDefault(ce => ce.PidUri.AbsoluteUri == pidUri.AbsoluteUri);
            return colidEntry != null;
        }
    }
}
