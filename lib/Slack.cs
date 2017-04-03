using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Slackbot
{
    class HelloRTMSession
    {
        public string url { get; set; }
        public bool Ok { get; set; }
        public string Error { get; set; }
    }

    class SlackUserList
    {
        public SlackUser[] Members;
    }

    class SlackUser
    {
        public string Id;
        public string Name;
    }

    public class AdvancedMessage
    {
        public AdvancedMessage()
        {
            As_User = true;
        }

        [JsonProperty(PropertyName = "as_user")]
        public bool As_User { get; }

        [JsonProperty(PropertyName = "channel")]
        public string Channel { get; set; }

        [JsonProperty(PropertyName = "attachments")]
        public List<Attachment> Attachments { get; set; }
    }

    public class Attachment
    {
        [JsonProperty(PropertyName = "fallback")]
        public string Fallback { get; set; }

        [JsonProperty(PropertyName = "color")]
        public string Color { get; set; }

        [JsonProperty(PropertyName = "pretext")]
        public string Pretext { get; set; }

        [JsonProperty(PropertyName = "author_name")]
        public string Author_Name { get; set; }

        [JsonProperty(PropertyName = "author_link")]
        public string Author_Link { get; set; }

        [JsonProperty(PropertyName = "author_icon")]
        public string Author_Icon { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "title_link")]
        public string Title_Link { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "fields")]
        public List<Field> Fields { get; set; }

        [JsonProperty(PropertyName = "image_url")]
        public string Image_Url { get; set; }

        [JsonProperty(PropertyName = "thumb_url")]
        public string Thumb_Url { get; set; }

        [JsonProperty(PropertyName = "footer")]
        public string Footer { get; set; }

        [JsonProperty(PropertyName = "footer_icon")]
        public string Footer_Icon { get; set; }

        [JsonProperty(PropertyName = "ts")]
        public int ts { get; set; }
    }

    public class Field
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        [JsonProperty(PropertyName = "short")]
        public bool Short { get; set; }
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

        public static async Task<string> GetUsername(string token, string userId)
        {
            try
            {
                var uri = $"https://slack.com/api/users.list?token={token}";

                using (var client = new HttpClient())
                using (var response = await client.GetAsync(uri))
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return Newtonsoft
                        .Json
                        .JsonConvert
                        .DeserializeObject<SlackUserList>(responseContent)
                        .Members.First(member => member.Id == userId.Split('|')[0])?.Name ?? string.Empty;
                }
            }
            catch (System.Exception)
            {
                return string.Empty;
            }

        }

        public static async Task<OnMessageArgs> SendMessageAsync(string token, AdvancedMessage message)
        {
            var json = JsonConvert.SerializeObject(message.Attachments, Formatting.None, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            var attachments = WebUtility.UrlEncode(json);

            var uri = $"https://slack.com/api/chat.postMessage?token={token}&channel={message.Channel}&as_user={message.As_User}&attachments={attachments}";

            using (var client = new HttpClient())
            {
                //var content = new StringContent(jsonBody, Encoding.UTF8, "text/plain");
                return await client.PostAsync(uri, null).ContinueWith(s => HandleErrorsAsync<OnMessageArgs>(s.Result)).Result;
            }
        }

        private static async Task<TResult> HandleErrorsAsync<TResult>(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var jobj = JObject.Parse(responseContent);

            if (response.IsSuccessStatusCode && jobj.SelectToken("$.ok").Value<bool>())
                return jobj.ToObject<TResult>();

            throw new HttpRequestException(responseContent);
        }
    }
}