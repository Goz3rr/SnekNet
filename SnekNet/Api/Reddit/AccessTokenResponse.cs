namespace SnekNet.Api.Reddit
{
    public class AccessTokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int? expires_in { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }
        public string device_id { get; set; }

        public string message { get; set; }
        public int? error { get; set; }
    }
}