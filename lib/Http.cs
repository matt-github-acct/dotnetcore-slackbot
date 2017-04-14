using System.Net.Http;
using System.Threading.Tasks;

namespace Slackbot
{
    public class Http
    {
        public async Task<HttpGetResult> Get(string uri)
        {
            var result = await new HttpClient().GetAsync(uri);
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