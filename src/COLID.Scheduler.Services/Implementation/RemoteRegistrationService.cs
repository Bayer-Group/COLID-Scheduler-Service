using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using COLID.Identity.Extensions;
using COLID.Identity.Services;
using COLID.Scheduler.Services.Configuration;
using COLID.Scheduler.Services.Interfaces;
using CorrelationId.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace COLID.Scheduler.Services.Implementation
{
    internal class RemoteRegistrationService : IRemoteRegistrationService
    {
        private readonly CancellationToken _cancellationToken;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly ICorrelationContextAccessor _correlationContext;
        private readonly ITokenService<RegistrationServiceTokenOptions> _tokenService;
        private readonly ILogger<RemoteRegistrationService> _logger;

        private readonly string _appRegistrationServiceNotifyInvaildDistributionEndpointApi;
        private readonly string _appRegistrationServiceNotifyInvalidContacts;
        private readonly string _appRegistrationServiceStoreFlagsInElastic;
        private readonly string _appRegistrationServiceNotifyForDueReviewsApi;

        public RemoteRegistrationService(
            IHttpClientFactory clientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ICorrelationContextAccessor correlationContext,
            ITokenService<RegistrationServiceTokenOptions> tokenService,
            ILogger<RemoteRegistrationService> logger)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            _tokenService = tokenService;
            _correlationContext = correlationContext;
            _cancellationToken = httpContextAccessor?.HttpContext?.RequestAborted ?? CancellationToken.None;
            _logger = logger;

            var serverUrl = _configuration.GetConnectionString("RegistrationServiceUrl");
            _appRegistrationServiceNotifyInvaildDistributionEndpointApi = $"{serverUrl}/api/v3/resourceDataValidityCheck/checkDistributionEndpoints";
            _appRegistrationServiceNotifyInvalidContacts = $"{serverUrl}/api/v3/resourceDataValidityCheck/checkDataStewardsAndDistributionEndpointContacts";
            _appRegistrationServiceStoreFlagsInElastic = $"{serverUrl}/api/v3/resourceDataValidityCheck/setInvalidResourceDataInElastic";
            _appRegistrationServiceNotifyForDueReviewsApi = $"{serverUrl}/api/v3/resource";
        }
        public async Task CheckDistributionEndpointValidityAndNotifyUsersAsync()
        {
            using (var httpClient = _clientFactory.CreateClient())
            {
                var response = await httpClient.SendRequestWithOptionsAsync(
                    HttpMethod.Get, _appRegistrationServiceNotifyInvaildDistributionEndpointApi,string.Empty,
                    await _tokenService.GetAccessTokenForWebApiAsync().ConfigureAwait(false), _cancellationToken, _correlationContext.CorrelationContext);

                response.EnsureSuccessStatusCode();

                _logger.LogInformation("remote registration api called for checking invalid distribution endpoint(s)");
            }
        }

        public async Task<Dictionary<string, string>> NotifyDataStewardsForDueResourceReview()
        {
            using (var httpClient = _clientFactory.CreateClient())
            {
                var notifyForDueReviewsApi = this._appRegistrationServiceNotifyForDueReviewsApi + "/notifyDueReviews";
                _logger.LogInformation("remote registration api called for notify users for due resources");
                var response = await httpClient.SendRequestWithOptionsAsync(
                    HttpMethod.Put, notifyForDueReviewsApi, string.Empty,
                    await _tokenService.GetAccessTokenForWebApiAsync().ConfigureAwait(false), _cancellationToken, _correlationContext.CorrelationContext);

                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync();

                Dictionary<string, string> emailList = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResponse);
                return emailList;
            }
        }

        public async Task CheckDataStewardsAndDistributionEndpointContactsAndNotifyUsers()
        {
            using (var httpClient = _clientFactory.CreateClient())
            {
                _logger.LogInformation("remote registration api called for checking invalid data stewards and distribution endpoint contacts");
                var response = await httpClient.SendRequestWithOptionsAsync(
                    HttpMethod.Get, _appRegistrationServiceNotifyInvalidContacts, string.Empty,
                    await _tokenService.GetAccessTokenForWebApiAsync().ConfigureAwait(false), _cancellationToken, _correlationContext.CorrelationContext);

                response.EnsureSuccessStatusCode();
            }
        }

        public async Task SetBrokenFlagsInElastic()
        {
            using (var httpClient = _clientFactory.CreateClient())
            {
                _logger.LogInformation("remote registration api called for setting broken flags in elastic");
                var response = await httpClient.SendRequestWithOptionsAsync(
                    HttpMethod.Get, _appRegistrationServiceStoreFlagsInElastic, string.Empty,
                    await _tokenService.GetAccessTokenForWebApiAsync().ConfigureAwait(false), _cancellationToken, _correlationContext.CorrelationContext);

                response.EnsureSuccessStatusCode();
            }
        }
    }
}
