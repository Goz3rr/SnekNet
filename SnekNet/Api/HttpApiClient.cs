using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Rest.TransientFaultHandling;

namespace SnekNet.Api
{
    internal class ApiErrorDetectionStrategy : ITransientErrorDetectionStrategy
    {
        public bool IsTransient(Exception ex)
        {
            return ex is TaskCanceledException || ex is RetryException;
        }
    }

    internal class RetryException : Exception { }

    public class HttpApiClient : IApiClient
    {
        private static readonly RetryPolicy retryPolicy = new RetryPolicy<ApiErrorDetectionStrategy>(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));

        public async Task<string> GetJSON(string url, string auth = null)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(15);
                client.DefaultRequestHeaders.Connection.Add("close");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", "SnekNet Backend v0.1");

                if (auth != null)
                {
                    client.DefaultRequestHeaders.Add("Authorization", auth);
                }

                return await retryPolicy.ExecuteAsync(async () =>
                {
                    using (var response = await client.GetAsync(url))
                    {
                        return await GetResponseBody(response);
                    }
                });
            }
        }

        public async Task<string> PostJSON(string url, HttpContent content, string auth = null)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(15);
                client.DefaultRequestHeaders.Connection.Add("close");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (auth != null)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(auth)));
                }

                return await retryPolicy.ExecuteAsync(async () =>
                {
                    using (var response = await client.PostAsync(url, content))
                    {
                        return await GetResponseBody(response);
                    }
                });
            }
        }

        public async Task<string> PostJSONWithToken(string url, HttpContent content, string token)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(15);
                client.DefaultRequestHeaders.Connection.Add("close");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", "SnekNet Backend v0.1");
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                return await retryPolicy.ExecuteAsync(async () =>
                {
                    using (var response = await client.PostAsync(url, content))
                    {
                        return await GetResponseBody(response);
                    }
                });
            }
        }

        private async Task<string> GetResponseBody(HttpResponseMessage response)
        {
            /*
            if (!response.IsSuccessStatusCode)
            {
                throw new RetryException();
            }
            */

            var body = await response.Content.ReadAsStringAsync();

            /*
            if (String.IsNullOrEmpty(body))
            {
                throw new RetryException();
            }
            */

            return body;
        }
    }
}
