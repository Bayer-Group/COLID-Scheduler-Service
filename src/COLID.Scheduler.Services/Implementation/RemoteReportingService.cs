using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using COLID.Identity.Extensions;
using COLID.Identity.Services;
using COLID.Scheduler.Common.DataModels;
using COLID.Scheduler.Services.Configuration;
using COLID.Scheduler.Services.Interface;
using CorrelationId.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace COLID.Scheduler.Services.Implementation
{
    internal class RemoteReportingService : IRemoteReportingService
    {
        private readonly CancellationToken _cancellationToken;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly ICorrelationContextAccessor _correlationContext;
        private readonly ITokenService<ReportingServiceTokenOptions> _tokenService;
        private readonly ILogger<RemoteReportingService> _logger;

        private readonly string _contactEndpoint;
        private readonly string _statisticsEndpoint;

        public RemoteReportingService(
            IHttpClientFactory clientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ICorrelationContextAccessor correlationContext,
            ITokenService<ReportingServiceTokenOptions> tokenService,
            ILogger<RemoteReportingService> logger)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            _tokenService = tokenService;
            _correlationContext = correlationContext;
            _cancellationToken = httpContextAccessor?.HttpContext?.RequestAborted ?? CancellationToken.None;
            _logger = logger;

            var serverUrl = _configuration.GetConnectionString("ReportingServiceUrl");
            _contactEndpoint = $"{serverUrl}/api/contact";
            _statisticsEndpoint = $"{serverUrl}/api/statistics";
        }

        public async Task<IEnumerable<string>> GetContactsFromAllColidEntries()
        {
            using var httpClient = _clientFactory.CreateClient();
            HttpResponseMessage response = null;
            try
            {
                var accessToken = await _tokenService.GetAccessTokenForWebApiAsync();
                response = await httpClient.SendRequestWithOptionsAsync(HttpMethod.Get, _contactEndpoint,
                    null, accessToken, _cancellationToken, _correlationContext.CorrelationContext);
                response.EnsureSuccessStatusCode();
            }
            catch (AuthenticationException ex)
            {
                _logger.LogError("An error occured", ex);
            }
            catch (HttpRequestException ex)
            {
                // No connection possible
                _logger.LogError("Couldn't connect to the remote reporting service.", ex);
            }

            if (response?.Content != null)
            {
                var responseContent = await response.Content.ReadAsAsync<IList<string>>();
                return responseContent;
            }

            return new List<string>();
        }

        public async Task<IEnumerable<ColidEntryContactsDto>> GetContactReferencedEntries(string userEmailAddress)
        {
            using var httpClient = _clientFactory.CreateClient();
            HttpResponseMessage response = null;
            try
            {
                var path = $"{_contactEndpoint}/{userEmailAddress}/colidEntries";
                var accessToken = await _tokenService.GetAccessTokenForWebApiAsync();
                response = await httpClient.SendRequestWithOptionsAsync(HttpMethod.Get, path, null, accessToken, _cancellationToken, _correlationContext.CorrelationContext);
                response.EnsureSuccessStatusCode();
            }
            catch (AuthenticationException ex)
            {
                _logger.LogError("An error occured", ex);
            }
            catch (HttpRequestException ex)
            {
                // No connection possible
                _logger.LogError("Couldn't connect to the remote reporting service.", ex);
            }

            if (response?.Content != null)
            {
                var responseContent = await response.Content.ReadAsAsync<IList<ColidEntryContactsDto>>();
                return responseContent;
            }

            return new List<ColidEntryContactsDto>();
        }

        public async Task<IEnumerable<PropertyCharacteristicDto>> GetConsumerGroupCharacteristics()
        {
            using var httpClient = _clientFactory.CreateClient();
            HttpResponseMessage response = null;
            try
            {
                var path = $"{_statisticsEndpoint}/resource/characteristics/consumergroup";
                var accessToken = await _tokenService.GetAccessTokenForWebApiAsync();
                response = await httpClient.SendRequestWithBearerTokenAsync(HttpMethod.Get, path,
                    null, accessToken, _cancellationToken);
                response.EnsureSuccessStatusCode();
            }
            catch (AuthenticationException ex)
            {
                _logger.LogError("An error occured", ex);
            }
            catch (HttpRequestException ex)
            {
                // No connection possible
                _logger.LogError("Couldn't connect to the remote reporting service.", ex);
            }

            if (response?.Content != null)
            {
                var responseContent = await response.Content.ReadAsAsync<IList<PropertyCharacteristicDto>>();
                return responseContent;
            }

            return new List<PropertyCharacteristicDto>();
        }

        public async Task<IEnumerable<PropertyCharacteristicDto>> GetInformationClassificationCharacteristics()
        {
            using var httpClient = _clientFactory.CreateClient();
            HttpResponseMessage response = null;
            try
            {
                var path = $"{_statisticsEndpoint}/resource/characteristics/informationclassification";
                var accessToken = await _tokenService.GetAccessTokenForWebApiAsync();
                response = await httpClient.SendRequestWithBearerTokenAsync(HttpMethod.Get, path,
                    null, accessToken, _cancellationToken);
                response.EnsureSuccessStatusCode();
            }
            catch (AuthenticationException ex)
            {
                _logger.LogError("An error occured", ex);
            }
            catch (HttpRequestException ex)
            {
                // No connection possible
                _logger.LogError("Couldn't connect to the remote reporting service.", ex);
            }

            if (response?.Content != null)
            {
                var responseContent = await response.Content.ReadAsAsync<IList<PropertyCharacteristicDto>>();
                return responseContent;
            }

            return new List<PropertyCharacteristicDto>();
        }

        public async Task<IEnumerable<PropertyCharacteristicDto>> GetResourceTypeCharacteristics()
        {
            using var httpClient = _clientFactory.CreateClient();
            HttpResponseMessage response = null;
            try
            {
                var path = $"{_statisticsEndpoint}/resource/characteristics/type";
                var accessToken = await _tokenService.GetAccessTokenForWebApiAsync();
                response = await httpClient.SendRequestWithBearerTokenAsync(HttpMethod.Get, path,
                    null, accessToken, _cancellationToken);
                response.EnsureSuccessStatusCode();
            }
            catch (AuthenticationException ex)
            {
                _logger.LogError("An error occured", ex);
            }
            catch (HttpRequestException ex)
            {
                // No connection possible
                _logger.LogError("Couldn't connect to the remote reporting service.", ex);
            }

            if (response?.Content != null)
            {
                var responseContent = await response.Content.ReadAsAsync<IList<PropertyCharacteristicDto>>();
                return responseContent;
            }

            return new List<PropertyCharacteristicDto>();
        }

        public async Task<IEnumerable<PropertyCharacteristicDto>> GetLifecycleStatusCharacteristics()
        {
            using var httpClient = _clientFactory.CreateClient();
            HttpResponseMessage response = null;
            try
            {
                var path = $"{_statisticsEndpoint}/resource/characteristics/lifecyclestatus";
                var accessToken = await _tokenService.GetAccessTokenForWebApiAsync();
                response = await httpClient.SendRequestWithBearerTokenAsync(HttpMethod.Get, path,
                    null, accessToken, _cancellationToken);
                response.EnsureSuccessStatusCode();
            }
            catch (AuthenticationException ex)
            {
                _logger.LogError("An error occured", ex);
            }
            catch (HttpRequestException ex)
            {
                // No connection possible
                _logger.LogError("Couldn't connect to the remote reporting service.", ex);
            }

            if (response?.Content != null)
            {
                var responseContent = await response.Content.ReadAsAsync<IList<PropertyCharacteristicDto>>();
                return responseContent;
            }

            return new List<PropertyCharacteristicDto>();
        }
    }
}
