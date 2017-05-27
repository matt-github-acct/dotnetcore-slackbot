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
        public static async Task<string> GetWebsocketUrl(string token) =>
            await new NewSlack(new Http()).GetWebsocketUrl(token);

        public static async Task<string> GetUsername(string token, string userId) =>
            await new NewSlack(new Http()).GetUsername(token, userId);
    }
}