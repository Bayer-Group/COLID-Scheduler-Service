using System;
using System.Threading;
using System.Threading.Tasks;
using COLID.Scheduler.Common.Constants;
using COLID.Scheduler.Jobs.Interface;
using COLID.SchedulerService.Jobs.Interface;
using Hangfire;
using Microsoft.Extensions.Logging;
using COLID.StatisticsLog.Services;
using COLID.Scheduler.Services.Interface;
using COLID.Scheduler.Common.DataModels;
using System.Collections.Generic;
using System.Linq;

namespace COLID.SchedulerService.Jobs.Implementation
{
    public class ResourceStatisticsJob : IResourceStatisticsJob
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ILogger<ResourceStatisticsJob> _logger;
        private readonly IRemoteReportingService _remoteReportingService;
        private readonly IGeneralLogService _statisticsLogService;

        public ResourceStatisticsJob(IBackgroundJobClient backgroundJobClient, ILogger<ResourceStatisticsJob> logger, IRemoteReportingService remoteReportingService, IGeneralLogService statisticsLogService)
        {
            _backgroundJobClient = backgroundJobClient;
            _logger = logger;
            _remoteReportingService = remoteReportingService;
            _statisticsLogService = statisticsLogService;

        }

        [Queue(Queue.Beta)]
        public async Task ExecuteAsync(CancellationToken token)
        {
            // Fetch Consumer Group Statistics and Log the information
            var consumerGroupCharacteristics = await _remoteReportingService.GetConsumerGroupCharacteristics();
            var consumerGroupStatistics = GetStatisticsByBasicProperty(consumerGroupCharacteristics.ToList());

            WriteStatisticsToLog("Resources_ByConsumerGroup", new Dictionary<string, dynamic> { { Metadata.ConsumerGroup, consumerGroupStatistics } });


            // Fetch Information Classification Statistics and Log the information
            var informationClassificationCharacteristics = await _remoteReportingService.GetInformationClassificationCharacteristics();
            var informationClassificationStatistics = GetStatisticsByBasicProperty(informationClassificationCharacteristics.ToList());
            WriteStatisticsToLog("Resources_ByInformationClassification", new Dictionary<string, dynamic> { { Metadata.InformationClassification, informationClassificationStatistics } });


            // Fetch Resource Type Characteristics and Log the information
            var resourceTypeCharacteristics = await _remoteReportingService.GetResourceTypeCharacteristics();
            var resourceTypeStatistics = GetStatisticsByBasicProperty(resourceTypeCharacteristics.ToList());
            WriteStatisticsToLog("Resources_ByType", new Dictionary<string, dynamic> { { Metadata.W3Type, resourceTypeStatistics } });

            // Fetch Lifecycle Status Characteristics and Log the information
            var lifecycleStatusCharacteristics = await _remoteReportingService.GetLifecycleStatusCharacteristics();
            var lifecycleStatusStatistics = GetStatisticsByBasicProperty(lifecycleStatusCharacteristics.ToList());
            WriteStatisticsToLog("Resources_ByLifecycleStatus", new Dictionary<string, dynamic> { { Metadata.LifecyleStatus, lifecycleStatusStatistics } });
        }

        /// <summary>
        /// Combines the consumer statistics with key, name and draft/published status counts into a message dictionary
        /// these statistics are written into elastic search as additional information
        /// </summary>
        /// <param name="propertyCharacteristicsDto">list of statistics received from reporting service</param>
        /// <returns>dictionary with consumer statistics</returns>
        private Dictionary<string, dynamic> GetStatisticsByBasicProperty(List<PropertyCharacteristicDto> propertyCharacteristicsDto)
        {
            var result = new Dictionary<string, dynamic>();
            foreach (var propertyCharacteristics in propertyCharacteristicsDto)
            {
                var key = propertyCharacteristics.Key;
                var propertyLabel = propertyCharacteristics.Name;
                var resourceDraftCount = propertyCharacteristics.DraftCount;
                var resourcePublishedCount = propertyCharacteristics.PublishedCount;

                result.Add(key, new Dictionary<string, dynamic>
                {{ "label", propertyLabel }, {  "resourceDraftCount", Convert.ToInt32(resourceDraftCount) },
                 { "resourcePublishedCount", Convert.ToInt32(resourcePublishedCount) }});

            }
            return result;
        }

        /// <summary>
        /// Writes the consumer statistics to elasticsearch
        /// </summary>
        /// <param name="message">statistics name (ConsumerGroup, LifecycleStatus etc.)</param>
        /// <param name="statistics">consumer resource statistics with counts</param>
        private void WriteStatisticsToLog(String message, Dictionary<string, dynamic> statistics)
        {
            if (statistics.Any())
            {
                _statisticsLogService.Info(message, statistics);
            }
        }
    }
}
