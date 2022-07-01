using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using COLID.Identity.Extensions;
using COLID.Identity.Services;
using COLID.Scheduler.Services.Configuration;
using COLID.Scheduler.Services.Interface;
using CorrelationId.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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
            _appRegistrationServiceNotifyInvaildDistributionEndpointApi = $"{serverUrl}/api/EndpointTest";
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
    }
}
