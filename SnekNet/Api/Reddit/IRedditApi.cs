using System.Threading.Tasks;

namespace SnekNet.Api.Reddit
{
    public interface IRedditApi
    {
        Task<AccessTokenResponse> GetAccesToken(string code);
        Task<AccessTokenResponse> RefreshAccessToken(string refreshcode);

        Task<MeResponse> GetUserData(string token);

        Task<string> UnlockCircle(string token, string id, string key);
        Task<string> JoinCircle(string token, string id);
    }
}
