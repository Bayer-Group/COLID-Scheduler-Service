using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ITokenService<AppDataServiceTokenOptions> _tokenService;
        private readonly ILogger<RemoteAppDataService> _logger;

        private readonly string _appDataServiceUserApi;
        private readonly string _appDataServiceSchedulerApi;
        private readonly string _activeDirectoryStatusApi;
        private readonly string _appDataServiceColidEntriesInvalidUserApi;

        public RemoteAppDataService(
            IHttpClientFactory clientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ITokenService<AppDataServiceTokenOptions> tokenService,
            ILogger<RemoteAppDataService> logger)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            _tokenService = tokenService;
            _cancellationToken = httpContextAccessor?.HttpContext?.RequestAborted ?? CancellationToken.None;
            _logger = logger;

            var serverUrl = _configuration.GetConnectionString("AppDataServiceUrl");
            _appDataServiceUserApi = $"{serverUrl}/api/users";
            _appDataServiceSchedulerApi = $"{serverUrl}/api/scheduler";
            _activeDirectoryStatusApi = $"{serverUrl}/api/activeDirectory/users/status";
            _appDataServiceColidEntriesInvalidUserApi = $"{serverUrl}/api/colidEntries/invalidUser";
        }

        public async void GetUsers()
        {
            using (var httpClient = _clientFactory.CreateClient())
            {
                var reqBody = string.Empty;

                var response = httpClient.SendRequestWithBearerTokenAsync(
                    HttpMethod.Post, _appDataServiceUserApi, reqBody,
                    await _tokenService.GetAccessTokenForWebApiAsync().ConfigureAwait(false), _cancellationToken).GetAwaiter().GetResult();

                response.EnsureSuccessStatusCode();
            }
        }

        public async Task<IList<Message>> GetMessagesToSend()
        {
            using (var httpClient = _clientFactory.CreateClient())
            {
                var response = httpClient.SendRequestWithBearerTokenAsync(
                    HttpMethod.Get, $"{_appDataServiceSchedulerApi}/messages/toSend", string.Empty,
                    await _tokenService.GetAccessTokenForWebApiAsync().ConfigureAwait(false), _cancellationToken).GetAwaiter().GetResult();

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

                var response = httpClient.SendRequestWithBearerTokenAsync(
                    HttpMethod.Put, path, string.Empty,
                    await _tokenService.GetAccessTokenForWebApiAsync().ConfigureAwait(false), _cancellationToken).GetAwaiter().GetResult();

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
                response = await httpClient.SendRequestWithBearerTokenAsync(HttpMethod.Post, _activeDirectoryStatusApi, adUserEmailSet, accessToken, _cancellationToken);
                _logger.LogDebug($"response={response}");
                response.EnsureSuccessStatusCode();
            }
            catch (AuthenticationException ex)
            {
                _logger.LogError("An error occured during authentication: ", ex);
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
            var path = $"{_appDataServiceColidEntriesInvalidUserApi}";

            try
            {
                var accessToken = await _tokenService.GetAccessTokenForWebApiAsync();
                var response = await httpClient.SendRequestWithBearerTokenAsync(HttpMethod.Put, path, content, accessToken, _cancellationToken);
                response.EnsureSuccessStatusCode();
            }
            catch (AuthenticationException ex)
            {
                _logger.LogError("An error occured during authentication: ", ex);
            }
        }
    }
}
