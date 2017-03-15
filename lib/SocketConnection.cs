using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Slackbot
{
    class SocketConnection
    {
        readonly Func<Task<string>> GetWebsocketUrl;
        public event EventHandler<string> OnData;
        private ClientWebSocket Socket;

        public SocketConnection(Func<Task<string>> getWebSocketUrl)
        {
            this.GetWebsocketUrl = getWebSocketUrl;
            Connect();
        }

        public async void SendData(ArraySegment<byte> data)
        {
            await Socket.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        async void Connect()
        {
            await TryConnect();
        }

        private async System.Threading.Tasks.Task TryConnect()
        {
            int retryCounter = 0;
            int maxRetryCount = 4;
            int secondsBetweenRetry = 2;

            while (retryCounter < maxRetryCount)
            {
                try
                {
                    Socket = new System.Net.WebSockets.ClientWebSocket();
                    await Socket.ConnectAsync(new Uri(await this.GetWebsocketUrl()), CancellationToken.None);

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
                    break;
                }
                catch (Exception e)
                {
                    int sleepTimeSeconds = Convert.ToInt32(Math.Pow(secondsBetweenRetry, retryCounter + 1));
                    retryCounter++;

                    if (retryCounter >= maxRetryCount)
                    {
                        throw e;
                    }
                    else
                    {
                        Thread.Sleep(sleepTimeSeconds * 1000);
                    }
                }
            }
        }
    }
}