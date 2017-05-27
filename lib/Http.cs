using System.Net.Http;
using System.Threading.Tasks;

namespace Slackbot
{
    public interface IHttp {
        Task<HttpGetResult> Get(string uri);
    }

    public class Http : IHttp
    {
        public async Task<HttpGetResult> Get(string uri)
        {
            HttpResponseMessage result;
            try
            {
                result = await new HttpClient().GetAsync(uri);
            }
            catch (System.Exception ex)
            {
                throw new System.Exception($"Error executing get request (uri: ${uri})", ex);
            }

            return new HttpGetResult
            {
                StatusCode = (int)result.StatusCode,
                Body = await result.Content.ReadAsStringAsync()
            };
        }
    }

    public class HttpGetResult
    {
        public int StatusCode { get; set; }
        public string Body { get; set; }
    }
}