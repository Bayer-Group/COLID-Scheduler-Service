using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using COLID.Identity.Extensions;
using COLID.Identity.Services;
using COLID.Scheduler.Common.Constants;
using COLID.Scheduler.Common.DataModels;
using COLID.Scheduler.Services.Configuration;
using COLID.Scheduler.Services.Interface;
using CorrelationId.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nest;
using Newtonsoft.Json.Linq;

namespace COLID.Scheduler.Services.Implementation
{
    internal class RemoteSearchService : IRemoteSearchService
    {
        private readonly CancellationToken _cancellationToken;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly ICorrelationContextAccessor _correlationContext;
        private readonly ITokenService<SearchServiceTokenOptions> _tokenService;
        private readonly ILogger<RemoteSearchService> _logger;

        private readonly string _userEndpoint;
        private readonly string _indexEndpoint;

        public RemoteSearchService(
            IHttpClientFactory clientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ICorrelationContextAccessor correlationContext,
            ITokenService<SearchServiceTokenOptions> tokenService,
            ILogger<RemoteSearchService> logger
            )
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            _tokenService = tokenService;
            _correlationContext = correlationContext;
            _cancellationToken = httpContextAccessor?.HttpContext?.RequestAborted ?? CancellationToken.None;
            _logger = logger;

            var serverUrl = _configuration.GetConnectionString("SearchServiceUrl");
            _userEndpoint = $"{serverUrl}/api/User";
        }

        public async Task WriteUsersToIndex(int appId)
        {
            using var httpClient = _clientFactory.CreateClient();
            HttpResponseMessage response = null;
            try
            {
                var path = $"{_userEndpoint}/writepiddmpuniqueusers?appName={appId}";
                var accessToken = await _tokenService.GetAccessTokenForWebApiAsync();
                response = await httpClient.SendRequestWithBearerTokenAsync(HttpMethod.Post, path,
                    null, accessToken, _cancellationToken);
                response.EnsureSuccessStatusCode();
            }
            catch (AuthenticationException ex)
            {
                _logger.LogError("An error occured", ex);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Couldn't connect to the remote search service.", ex);
            }
        }

        public async Task WriteSubscribedSearchFiltersCountToIndex(Dictionary<string, int> allSavedSearchFilters)
        {
            using var httpClient = _clientFactory.CreateClient();
            HttpResponseMessage response = null;
            try
            {
                var path = $"{_userEndpoint}/writeAllSavedSearchFiltersCountToLogs";
                var accessToken = await _tokenService.GetAccessTokenForWebApiAsync();
                response = await httpClient.SendRequestWithBearerTokenAsync(HttpMethod.Post, path,
                    allSavedSearchFilters, accessToken, _cancellationToken);
                response.EnsureSuccessStatusCode();
            }
            catch (AuthenticationException ex)
            {
                _logger.LogError("An error occured", ex);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Couldn't connect to the remote search service.", ex);
            }
        }

        public async Task WriteAllFavoritesListToIndex(Dictionary<string, int> allFavoritesList)
        {
            using var httpClient = _clientFactory.CreateClient();
            HttpResponseMessage response = null;
            try
            {
                var path = $"{_userEndpoint}/writeFavoritesListCountToLogs";
                var accessToken = await _tokenService.GetAccessTokenForWebApiAsync();
                response = await httpClient.SendRequestWithBearerTokenAsync(HttpMethod.Post, path,
                    allFavoritesList, accessToken, _cancellationToken);
                response.EnsureSuccessStatusCode();
            }
            catch (AuthenticationException ex)
            {
                _logger.LogError("An error occured", ex);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Couldn't connect to the remote search service.", ex);
            }
        }

        public async Task WriteAllSubscriptionsCountToIndex(Dictionary<string, int> allSubscriptions)
        {
            using var httpClient = _clientFactory.CreateClient();
            HttpResponseMessage response = null;
            try
            {
                var path = $"{_userEndpoint}/writeAllSubscriptionsCountToLogs";
                var accessToken = await _tokenService.GetAccessTokenForWebApiAsync();
                response = await httpClient.SendRequestWithBearerTokenAsync(HttpMethod.Post, path,
                    allSubscriptions, accessToken, _cancellationToken);
                response.EnsureSuccessStatusCode();
            }
            catch (AuthenticationException ex)
            {
                _logger.LogError("An error occured", ex);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Couldn't connect to the remote search service.", ex);
            }
        }
    }
}
