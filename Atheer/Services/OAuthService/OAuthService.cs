using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Atheer.Exceptions;
using Atheer.Utilities.Config.Models;
using Microsoft.Extensions.Options;

namespace Atheer.Services.OAuthService
{
    public class OAuthService : IOAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<GithubOAuth> _config;

        // TODO add retry
        public OAuthService(HttpClient httpClient, IOptions<GithubOAuth> config)
        {
            _httpClient = httpClient;
            _config = config;
            _httpClient.Timeout = TimeSpan.FromSeconds(5);
        }
        
        public async Task<OAuthUserInfo> GetUserInfo(OAuthProvider provider, string authCode)
        {
            string accessToken = await GetGithubAccessToken(authCode).ConfigureAwait(false);
            var userInfo = await GetGithubUser(accessToken).ConfigureAwait(false);

            return new OAuthUserInfo
            {
                Name = userInfo.name,
                Email = userInfo.email,
                OAuthProvider = provider.ToString(),
                OAuthUsername = userInfo.login
            };
        }

        private async Task<GithubUserResponse> GetGithubUser(string accessToken)
        {
            // https://docs.github.com/en/rest/reference/users
            var uri = "https://api.github.com/user";
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, uri);
            httpRequest.Headers.Add("Accept", "application/json");
            httpRequest.Headers.Add("Authorization", $"token {accessToken}");
            httpRequest.Headers.Add("User-Agent", "Atheer");

            var httpResponse = await _httpClient.SendAsync(httpRequest).ConfigureAwait(false);
            await using var stream = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var githubUserModel = await JsonSerializer.DeserializeAsync<GithubUserResponse>(stream).ConfigureAwait(false);

            return githubUserModel;
        }

        private async Task<string> GetGithubAccessToken(string code)
        {
            // https://docs.github.com/en/developers/apps/authorizing-oauth-apps#web-application-flow
            var sb = new StringBuilder(4);
            sb.Append("https://github.com/login/oauth/access_token")
                .Append($"?client_id={_config.Value.ClientId}")
                .Append($"&client_secret={_config.Value.ClientSecret}")
                .Append($"&code={code}");
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, sb.ToString());
            httpRequest.Headers.Add("Accept", "application/json");

            var httpResponse = await _httpClient.SendAsync(httpRequest).ConfigureAwait(false);
            await using var stream = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var json = await JsonDocument.ParseAsync(stream).ConfigureAwait(false);
            
            string accessTokenName = "access_token";
            var accessTokenPropExists = json.RootElement.TryGetProperty(accessTokenName, out var accessTokenProp);
            // This happens if reusing same code
            if (!accessTokenPropExists) throw new FailedOperationException();
            
            return accessTokenProp.GetString();
        }
    }
}