using System.Net.Http;
using System.Threading.Tasks;

namespace SnekNet.Api
{
    public interface IApiClient
    {
        Task<string> GetJSON(string url, string auth = null);
        Task<string> PostJSON(string url, HttpContent content, string auth = null);
    }
}
