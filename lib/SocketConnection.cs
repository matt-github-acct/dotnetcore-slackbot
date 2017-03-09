using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace Slackbot
{
    class SocketConnection
    {
        public readonly string Url;
        public event EventHandler<string> OnData;
        private ClientWebSocket Socket;

        public SocketConnection(string url)
        {
            this.Url = url;
            Connect();
        }

        public async void SendData(ArraySegment<byte> data){
            await Socket.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        async void Connect()
        {
            try
            {
                Socket = new System.Net.WebSockets.ClientWebSocket();
                await Socket.ConnectAsync(new Uri(this.Url), CancellationToken.None);

                var receiveBytes = new byte[4096];
                var receiveBuffer = new ArraySegment<byte>(receiveBytes);

                while (Socket.State == WebSocketState.Open)
                {
                    var receivedMessage = await Socket.ReceiveAsync(receiveBuffer, CancellationToken.None);
                    if (receivedMessage.MessageType == WebSocketMessageType.Close)
                    {
                        await
                            Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "closing websocket",
                                CancellationToken.None);
                    }
                    else
                    {
                        var messageBytes = receiveBuffer.Skip(receiveBuffer.Offset).Take(receivedMessage.Count).ToArray();

                        var rawMessage = new UTF8Encoding().GetString(messageBytes);
                        OnData.Invoke(this, rawMessage);
                    }
                }
            }
            catch (System.Exception)
            {
                Connect();
            }
        }
    }
}