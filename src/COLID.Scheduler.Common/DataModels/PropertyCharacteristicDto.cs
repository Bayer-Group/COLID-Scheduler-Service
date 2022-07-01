using System;
using System.Collections.Generic;
using System.Text;

namespace COLID.Scheduler.Common.DataModels
{
   public class PropertyCharacteristicDto
    {
        public string Key { get; set; }

        public int Count { get; set; }

        public string Name { get; set; }

        public int DraftCount { get; set; }

        public int PublishedCount { get; set; }

        public PropertyCharacteristicDto() { }

        public PropertyCharacteristicDto(string key, string count, string name) { }
    }

}
