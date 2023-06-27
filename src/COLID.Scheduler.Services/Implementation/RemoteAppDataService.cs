using System.Collections.Generic;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using COLID.Identity.Extensions;
using COLID.Identity.Services;
using COLID.Scheduler.Common.DataModels;
using COLID.Scheduler.Services.Configuration;
using COLID.Scheduler.Services.Interfaces;
using CorrelationId.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace COLID.Scheduler.Services.Implementation
{
    internal class RemoteAppDataService : IRemoteAppDataService
    {
        private readonly CancellationToken _cancellationToken;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly ICorrelationContextAccessor _correlationContext;
        private readonly ITokenService<AppDataServiceTokenOptions> _tokenService;
        private readonly ILogger<RemoteAppDataService> _logger;

        private readonly string _appDataServiceUserApi;
        private readonly string _appDataServiceMessagesApi;
        private readonly string _activeDirectoryStatusApi;
        private readonly string _appDataServiceColidEntries;

        public RemoteAppDataService(
            IHttpClientFactory clientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ICorrelationContextAccessor correlationContext,
            ITokenService<AppDataServiceTokenOptions> tokenService,
            ILogger<RemoteAppDataService> logger)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            _tokenService = tokenService;
            _correlationContext = correlationContext;
            _cancellationToken = httpContextAccessor?.HttpContext?.RequestAborted ?? CancellationToken.None;
            _logger = logger;

            var serverUrl = _configuration.GetConnectionString("AppDataServiceUrl");
            _appDataServiceUserApi = $"{serverUrl}/api/users";
            _appDataServiceMessagesApi = $"{serverUrl}/api/messages";
            _activeDirectoryStatusApi = $"{serverUrl}/api/activeDirectory/users/status";
            //_appDataServiceColidEntriesInvalidUserApi = $"{serverUrl}/api/colidEntries/invalidUser";
            _appDataServiceColidEntries = $"{serverUrl}/api/colidEntries";
        }

        public async void GetUsers()
        {
            using (var httpClient = _clientFactory.CreateClient())
            {
                var reqBody = string.Empty;

                var response = httpClient.SendRequestWithOptionsAsync(
                    HttpMethod.Post, _appDataServiceUserApi, reqBody,
                    await _tokenService.GetAccessTokenForWebApiAsync().ConfigureAwait(false), _cancellationToken, _correlationContext.CorrelationContext).GetAwaiter().GetResult();

                response.EnsureSuccessStatusCode();
            }
        }

        
        public async Task<IList<Message>> GetMessagesToSend()
        {
            using (var httpClient = _clientFactory.CreateClient())
            {
                var response = httpClient.SendRequestWithOptionsAsync(
                    HttpMethod.Get, $"{_appDataServiceMessagesApi}/toSend", string.Empty,
                    await _tokenService.GetAccessTokenForWebApiAsync().ConfigureAwait(false), _cancellationToken, _correlationContext.CorrelationContext).GetAwaiter().GetResult();

                response.EnsureSuccessStatusCode();

                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsAsync<IList<Message>>();
                    return responseContent;
                }

                return new List<Message>();
            }
        }

        public async void MarkMessageAsSent(string userId, int messageId)
        {
            using (var httpClient = _clientFactory.CreateClient())
            {
                var path = $"{_appDataServiceUserApi}/{userId}/messages/{messageId}/markSent";

                var response = httpClient.SendRequestWithOptionsAsync(
                    HttpMethod.Put, path, string.Empty,
                    await _tokenService.GetAccessTokenForWebApiAsync().ConfigureAwait(false), _cancellationToken, _correlationContext.CorrelationContext).GetAwaiter().GetResult();

                response.EnsureSuccessStatusCode();
            }
        }

        public async Task<IEnumerable<AdUserDto>> CheckUsersValidityAsync(IEnumerable<string> adUserEmailSet)
        {
            _logger.LogDebug($"adUserEmailSet={string.Join(",",adUserEmailSet)}");
            using var httpClient = _clientFactory.CreateClient();
            HttpResponseMessage response = null;
            try
            {
                var accessToken = await _tokenService.GetAccessTokenForWebApiAsync();
                _logger.LogDebug($"accessToken={accessToken}");
                response = await httpClient.SendRequestWithOptionsAsync(HttpMethod.Post, _activeDirectoryStatusApi, adUserEmailSet, accessToken, _cancellationToken, _correlationContext.CorrelationContext);
                _logger.LogDebug($"response={response}");
                response.EnsureSuccessStatusCode();
            }
            catch (AuthenticationException ex)
            {
                _logger.LogError("An error occurred during authentication: ", ex);
            }

            if (response.Content != null)
            {
                var responseContent = await response.Content.ReadAsAsync<HashSet<AdUserDto>>();
                return responseContent;
            }

            return new HashSet<AdUserDto>();
        }

        public async Task CreateMessagesOfInvalidUsersForContact(ColidEntryContactInvalidUsersDto content)
        {
            using var httpClient = _clientFactory.CreateClient();
            var path = $"{_appDataServiceColidEntries}/invalidUser";

            try
            {
                var accessToken = await _tokenService.GetAccessTokenForWebApiAsync();
                var response = await httpClient.SendRequestWithOptionsAsync(HttpMethod.Put, path, content, accessToken, _cancellationToken, _correlationContext.CorrelationContext);
                response.EnsureSuccessStatusCode();
            }
            catch (AuthenticationException ex)
            {
                _logger.LogError("An error occurred during authentication: ", ex);
            }
        }

        public async void DeleteExpiredMessages()
        {
            using (var httpClient = _clientFactory.CreateClient())
            {
                try
                {
                    var path = $"{_appDataServiceMessagesApi}";
                    var accessToken = await _tokenService.GetAccessTokenForWebApiAsync();
                    var response = await httpClient.SendRequestWithBearerTokenAsync(
                        HttpMethod.Delete, path, string.Empty, accessToken, _cancellationToken);
                    _logger.LogInformation($"retrieved a response");
                }
                catch (AuthenticationException ex)
                {
                    _logger.LogError("An error occurred", ex);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError("Couldn't connect to the remote reporting service.", ex);
                }

            }
        }

        public async void ProcessStoredQueries()
        {
            using (var httpClient = _clientFactory.CreateClient())
            {
                var path = $"{_appDataServiceUserApi}/processStoredQueries";
                var accessToken = await _tokenService.GetAccessTokenForWebApiAsync();
                HttpResponseMessage response = null;
                try
                {
                    response = await httpClient.SendRequestWithBearerTokenAsync(
                        HttpMethod.Post, path, string.Empty, accessToken, _cancellationToken);
                    _logger.LogInformation($"retrieved a response");
                }
                catch (AuthenticationException ex)
                {
                    _logger.LogError("An error occurred", ex);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError("Couldn't connect to the remote reporting service.", ex);
                }
            }
        }

        public async Task<Dictionary<string, int>> GetAllSubscribedSearchFiltersCountDMP()
        {
            using (var httpClient = _clientFactory.CreateClient())
            {
                var response = httpClient.SendRequestWithOptionsAsync(
                    HttpMethod.Get, $"{_appDataServiceUserApi}/allSubscribedSearchFiltersDataMarketplaceCount", string.Empty,
                    await _tokenService.GetAccessTokenForWebApiAsync().ConfigureAwait(false), _cancellationToken, _correlationContext.CorrelationContext).GetAwaiter().GetResult();

                response.EnsureSuccessStatusCode();

                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsAsync<Dictionary<string, int>>();
                    return responseContent;
                }

                return new Dictionary<string, int>();
            }
        }
        
        public async Task<Dictionary<string, int>> GetAllFavoritesListCount()
        {
            using (var httpClient = _clientFactory.CreateClient())
            {
                var response = httpClient.SendRequestWithOptionsAsync(
                    HttpMethod.Get, $"{_appDataServiceUserApi}/getAllFavoritesListCount", string.Empty,
                    await _tokenService.GetAccessTokenForWebApiAsync().ConfigureAwait(false), _cancellationToken, _correlationContext.CorrelationContext).GetAwaiter().GetResult();

                response.EnsureSuccessStatusCode();

                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsAsync<Dictionary<string, int>>();
                    return responseContent;
                }

                return new Dictionary<string, int>();
            }
        }

        public async Task<Dictionary<string, int>> GetAllSubscriptionsCount()
        {
            using (var httpClient = _clientFactory.CreateClient())
            {
                var response = httpClient.SendRequestWithOptionsAsync(
                    HttpMethod.Get, $"{_appDataServiceColidEntries}/GetAllSubscriptionsCount", string.Empty,
                    await _tokenService.GetAccessTokenForWebApiAsync().ConfigureAwait(false), _cancellationToken, _correlationContext.CorrelationContext).GetAwaiter().GetResult();

                response.EnsureSuccessStatusCode();

                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsAsync<Dictionary<string, int>>();
                    return responseContent;
                }

                return new Dictionary<string, int>();
            }
        }
    }
}
