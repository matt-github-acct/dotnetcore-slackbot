using System;
using System.Linq;
using System.Text;

namespace Slackbot
{
    public class OnMessageArgs
    {
        public string Text;
        public string Channel;
        public string[] MentionedUsers = new string[0];
        public string RawMessage;
        public string User;
    }

    class SlackData
    {
        public string Type;
    }

    public class Bot
    {
        public readonly string Token;
        public readonly string Username;

        SocketConnection SocketConnection;

        public event EventHandler<OnMessageArgs> OnMessage;

        public Bot(string token, string username)
        {
            this.Token = token;
            this.Username = username;

            Connect();
        }

        public void SendMessage(string channel, string message)
        {
            var outbound = new
            {
                id = Guid.NewGuid().ToString(),
                type = "message",
                channel = channel,
                text = message
            };

            var outboundBytes = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(outbound));
            var outboundBuffer = new ArraySegment<byte>(outboundBytes);

            SocketConnection.SendData(outboundBuffer);
        }

        async void Connect()
        {
            SocketConnection = new SocketConnection(async () => await Slack.GetWebsocketUrl(this.Token));

            SocketConnection.OnData += (sender, data) =>
            {
                HandleOnData(data);
            };
        }

        async void HandleOnData(string data)
        {
            var message = Newtonsoft.Json.JsonConvert.DeserializeObject<SlackData>(data);

            if (message.Type == "message")
            {
                var args = Newtonsoft.Json.JsonConvert.DeserializeObject<OnMessageArgs>(data);

                args.MentionedUsers = SlackMessage.FindMentionedUsers(data);

                for (var i = 0; i < args.MentionedUsers.Count(); i++)
                {
                    args.MentionedUsers[i] = await Slack.GetUsername(this.Token, args.MentionedUsers[i]);
                }

                args.User = await Slack.GetUsername(this.Token, args.User);

                args.RawMessage = data;

                OnMessage.Invoke(this, args);
            }
        }
    }
}
