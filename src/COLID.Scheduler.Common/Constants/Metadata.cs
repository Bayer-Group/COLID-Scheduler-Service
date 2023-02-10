using System.IO;
using Microsoft.Extensions.Configuration;

namespace COLID.Scheduler.Common.Constants
{
    public static class Metadata
    {
        private static readonly string _basePath = Path.GetFullPath("appsettings.json");
        private static readonly string _filePath = _basePath.Substring(0, _basePath.Length - 16);
        private static IConfigurationRoot _configuration = new ConfigurationBuilder()
                     .SetBasePath(_filePath)
                    .AddJsonFile("appsettings.json")
                    .Build();
        public static readonly string _serviceUrl = _configuration.GetValue<string>("ServiceUrl");

        public static readonly string Author = _serviceUrl + "kos/19050/author";

        public static readonly string LastChangeUser = _serviceUrl + "kos/19050/lastChangeUser";

        public static readonly string DataSteward = _serviceUrl + "kos/19050/hasDataSteward";

        public static readonly string ContactPerson = _serviceUrl + "kos/19050/hasContactPerson";

        public static readonly string ChangeRequester = _serviceUrl + "kos/19050/546454";

        public static readonly string ConsumerGroup = _serviceUrl + "kos/19050#hasConsumerGroup";

        public static readonly string InformationClassification = _serviceUrl + "kos/19050/hasInformationClassification";

        public const string W3Type = "http://www/w3/org/1999/02/22-rdf-syntax-ns#type";

        public static readonly string LifecyleStatus = _serviceUrl + "kos/19050/hasLifecycleStatus";

        public static readonly string ConsumerGroupContact = _serviceUrl + "kos/19050/hasConsumerGroupContactPerson";

    }
}
