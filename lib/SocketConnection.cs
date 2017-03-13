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
        private int maxRetryCount = 4;
        private int secondsBetweenRetry = 2;

        public SocketConnection(string url)
        {
            this.Url = url;
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
            while (retryCounter < maxRetryCount)
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
                    break;
                }
                catch (Exception e)
                {
                    int sleepTimeSeconds = Convert.ToInt32(Math.Pow(secondsBetweenRetry, retryCounter + 1));
                    retryCounter++;

                    if (retryCounter >= maxRetryCount)
                    {
                        Console.WriteLine($"FATAL exception connecting to Slack: {Environment.NewLine} {e.ToString()}");
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