using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Slackbot
{
    public class HelloRTMSession
    {
        public string url { get; set; }
        public bool Ok { get; set; }
        public string Error { get; set; }
    }

    public class SlackUserList
    {
        public SlackUser[] Members;
    }

    public class SlackUser
    {
        public string Id;
        public string Name;
    }

    static class Slack
    {
        public static async Task<string> GetWebsocketUrl(string token)
        {
            var uri = $"https://slack.com/api/rtm.start?token={token}";

            using (var client = new HttpClient())
            using (var response = await client.GetAsync(uri))
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var helloRTMSession = Newtonsoft.Json.JsonConvert.DeserializeObject<HelloRTMSession>(responseContent);

                return helloRTMSession.Ok 
                    ? Newtonsoft.Json.JsonConvert.DeserializeObject<HelloRTMSession>(responseContent).url 
                    : throw new Exception($"FATAL: connecting to Slack RTM failed ({helloRTMSession.Error})");
            }
        }

        public static async Task<string> GetUsername(string token, string userId) =>
            await new NewSlack(new Http()).GetUsername(token, userId);
    }
}