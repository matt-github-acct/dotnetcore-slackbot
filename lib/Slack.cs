using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SimpleSlackbot
{
    class HelloRTMSession
    {
        public string url { get; set; }
    }

    class SlackUserList{
        public SlackUser[] Members;
    }

    class SlackUser{
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
                return Newtonsoft.Json.JsonConvert.DeserializeObject<HelloRTMSession>(responseContent).url;
            }
        }

        public static async Task<string> GetUsername(string token, string userId){
            var uri = $"https://slack.com/api/users.list?token={token}";

            using (var client = new HttpClient())
            using (var response = await client.GetAsync(uri))
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return Newtonsoft
                    .Json
                    .JsonConvert
                    .DeserializeObject<SlackUserList>(responseContent)
                    .Members.First(member => member.Id == userId).Name;
            }
        }
    }
}