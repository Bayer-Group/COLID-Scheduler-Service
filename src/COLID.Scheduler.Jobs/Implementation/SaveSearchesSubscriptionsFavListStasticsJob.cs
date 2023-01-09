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
    public class SaveSearchesSubscriptionsFavListStasticsJob : ISaveSearchesSubscriptionsFavListStasticsJob
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ILogger<SaveSearchesSubscriptionsFavListStasticsJob> _logger;
        private readonly IRemoteAppDataService _remoteAppDataService;
        private readonly IRemoteSearchService _remoteSearchService;
        private readonly IGeneralLogService _statisticsLogService;

        public SaveSearchesSubscriptionsFavListStasticsJob(IBackgroundJobClient backgroundJobClient, ILogger<SaveSearchesSubscriptionsFavListStasticsJob> logger, IRemoteAppDataService remoteAppDataService, IGeneralLogService statisticsLogService, IRemoteSearchService remoteSearchService)
        {
            _backgroundJobClient = backgroundJobClient;
            _logger = logger;
            _remoteAppDataService = remoteAppDataService;
            _statisticsLogService = statisticsLogService;
            _remoteSearchService = remoteSearchService;
        }

        [Queue(Queue.Beta)]
        public async Task ExecuteAsync(CancellationToken token)
        {
            // Fetch Subscribed Search Filters Count Statistics and Log the information
            var allSavedSearchFilters = await _remoteAppDataService.GetAllSubscribedSearchFiltersCountDMP();
            await _remoteSearchService.WriteSubscribedSearchFiltersCountToIndex(allSavedSearchFilters);

            // Fetch Favorites List Count Statistics and Log the information
            var allFavoritesList = await _remoteAppDataService.GetAllFavoritesListCount();
            await _remoteSearchService.WriteAllFavoritesListToIndex(allFavoritesList);

            // Fetch All Subscriptions Count Statistics and Log the information
            var allSubscriptions = await _remoteAppDataService.GetAllSubscriptionsCount();
            await _remoteSearchService.WriteAllSubscriptionsCountToIndex(allSubscriptions);

        }
    }
}
