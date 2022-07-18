using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZeissMachineStream
{
    public class WebSocketsHelper

    {
        ClientWebSocket ws = null;
        Uri uri = null;
        bool isUserClose = false;

        public WebSocketState? State { get => ws?.State; }

        public delegate void MessageEventHandler(object sender, string data);
        public delegate void ErrorEventHandler(object sender, Exception ex);


        public event EventHandler OnOpen;

        public event MessageEventHandler OnMessage;

        public event ErrorEventHandler OnError;

        public event EventHandler OnClose;

        public WebSocketsHelper(string wsUrl)
        {
            uri = new Uri(wsUrl);
            ws = new ClientWebSocket();
            ws.Options.KeepAliveInterval = TimeSpan.FromSeconds(60);
        }


        public void Open()
        {
            Task.Run(async () =>
            {
                if (ws.State == WebSocketState.Connecting || ws.State == WebSocketState.Open)
                    return;

                string netErr = string.Empty;
                try
                {
                    isUserClose = false;
                    ws = new ClientWebSocket();
                    await ws.ConnectAsync(uri, CancellationToken.None);

                    if (OnOpen != null)
                        OnOpen(ws, new EventArgs());

                    List<byte> bs = new List<byte>();
                    var buffer = new byte[1024 * 4];

                    WebSocketReceiveResult result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    while (!result.CloseStatus.HasValue)
                    {
                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            bs.AddRange(buffer.Take(result.Count));


                            if (result.EndOfMessage)
                            {
                                string userMsg = Encoding.UTF8.GetString(bs.ToArray(), 0, bs.Count);

                                if (OnMessage != null)
                                    OnMessage(ws, userMsg);

                                bs = new List<byte>();
                            }
                        }

                        result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    }
                }
                catch (Exception ex)
                {
                    netErr = ex.Message;

                    if (OnError != null)
                        OnError(ws, ex);

                    if (ws != null && ws.State == WebSocketState.Open)

                        await ws.CloseAsync(WebSocketCloseStatus.Empty, ex.Message, CancellationToken.None);
                }
                finally
                {
                    if (!isUserClose)
                        Close(ws.CloseStatus.Value, ws.CloseStatusDescription + netErr);
                }
            });

        }

        public bool Send(string message)
        {
            if (ws.State != WebSocketState.Open)
                return false;

            Task.Run(async () =>
            {
                var replyMess = Encoding.UTF8.GetBytes(message);
                await ws.SendAsync(new ArraySegment<byte>(replyMess), WebSocketMessageType.Text, true, CancellationToken.None);
            });

            return true;
        }

        public bool Send(byte[] bytes)
        {
            if (ws.State != WebSocketState.Open)
                return false;

            Task.Run(async () =>
            {
                await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Binary, true, CancellationToken.None);
            });

            return true;
        }

        public void Close()
        {
            isUserClose = true;
            Close(WebSocketCloseStatus.NormalClosure, "用户手动关闭");
        }

        public void Close(WebSocketCloseStatus closeStatus, string statusDescription)
        {
            Task.Run(async () =>
            {
                try
                {
                    await ws.CloseAsync(closeStatus, statusDescription, CancellationToken.None);
                }
                catch (Exception ex)
                {

                }

                ws.Abort();
                ws.Dispose();

                if (OnClose != null)
                    OnClose(ws, new EventArgs());
            });
        }
    }
}
