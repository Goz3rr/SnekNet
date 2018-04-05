using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace SnekNet.Api.Reddit
{
    public class RedditApi : IRedditApi
    {
        public const string BaseUrl = "https://www.reddit.com/api/v1";
        public const string OAuthUrl = "https://oauth.reddit.com/api/v1";

        private readonly IConfiguration configuration;
        private readonly IApiClient apiClient;

        public RedditApi(IConfiguration configuration, IApiClient apiClient)
        {
            this.configuration = configuration;
            this.apiClient = apiClient;
        }

        public async Task<AccessTokenResponse> GetAccesToken(string code)
        {
            var json = await apiClient.PostJSON($"{BaseUrl}/access_token",
                new StringContent($"grant_type=authorization_code&code={code}&redirect_uri={configuration["Reddit:RedirectURI"]}", Encoding.UTF8, "application/x-www-form-urlencoded"),
                $"{configuration["Reddit:ClientID"]}:{configuration["Reddit:Secret"]}");

            return JsonConvert.DeserializeObject<AccessTokenResponse>(json);
        }

        public async Task<AccessTokenResponse> RefreshAccessToken(string refreshcode)
        {
            var json = await apiClient.PostJSON($"{BaseUrl}/access_token",
                new StringContent($"grant_type=refresh_token&refresh_token={refreshcode}", Encoding.UTF8, "application/x-www-form-urlencoded"),
                $"{configuration["Reddit:ClientID"]}:{configuration["Reddit:Secret"]}");

            return JsonConvert.DeserializeObject<AccessTokenResponse>(json);
        }

        public async Task<MeResponse> GetUserData(string token)
        {
            var json = await apiClient.GetJSON($"{OAuthUrl}/me", $"bearer {token}");

            return JsonConvert.DeserializeObject<MeResponse>(json);
        }

        public Task<string> UnlockCircle(string token, string id, string key)
        {
            return apiClient.PostJSONWithToken($"https://oauth.reddit.com/api/guess_voting_key.json", new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("id", $"t3_{id}"),
                new KeyValuePair<string, string>("vote_key", key),
                new KeyValuePair<string, string>("raw_json", "1")
            }), token);
        }

        public Task<string> JoinCircle(string token, string id)
        {
            return apiClient.PostJSONWithToken($"https://oauth.reddit.com/api/circle_vote.json?dir=1&id=t3_{id}", new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("id", $"t3_{id}"),
                new KeyValuePair<string, string>("dir", "1")
            }), token);
        }
    }
}
