using System.Threading.Tasks;

namespace SnekNet.Api.Reddit
{
    public interface IRedditApi
    {
        Task<AccessTokenResponse> GetAccesToken(string code);

        Task<MeResponse> GetUserData(string token);
    }
}
