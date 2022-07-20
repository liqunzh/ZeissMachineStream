using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZeissMachineStream.Helper
{
    public sealed class WebSocketsHelper

    {
        private ClientWebSocket _ws = null;
        private Uri _uri = null;
        private bool _isUserClose = false;

        public WebSocketState? State { get => _ws?.State; }

        public delegate void MessageEventHandler(object sender, string data);
        public delegate void ErrorEventHandler(object sender, Exception ex);


        public event EventHandler OnOpen;

        public event MessageEventHandler OnMessage;

        public event ErrorEventHandler OnError;

        public event EventHandler OnClose;

        public WebSocketsHelper(string wsUrl)
        {
            _uri = new Uri(wsUrl);
            _ws = new ClientWebSocket();
            _ws.Options.KeepAliveInterval = TimeSpan.FromSeconds(60);
        }


        public void Open()
        {
            Task.Run(async () =>
            {
                if (_ws.State == WebSocketState.Connecting || _ws.State == WebSocketState.Open)
                    return;

                string netErr = string.Empty;
                try
                {
                    _isUserClose = false;
                    _ws = new ClientWebSocket();
                    await _ws.ConnectAsync(_uri, CancellationToken.None);

                    if (OnOpen != null)
                        OnOpen(_ws, new EventArgs());

                    List<byte> bs = new List<byte>();
                    var buffer = new byte[1024 * 4];

                    WebSocketReceiveResult result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    while (!result.CloseStatus.HasValue)
                    {
                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            bs.AddRange(buffer.Take(result.Count));


                            if (result.EndOfMessage)
                            {
                                string userMsg = Encoding.UTF8.GetString(bs.ToArray(), 0, bs.Count);

                                if (OnMessage != null)
                                    OnMessage(_ws, userMsg);

                                bs = new List<byte>();
                            }
                        }

                        result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    }
                }
                catch (Exception ex)
                {
                    netErr = ex.Message;

                    if (OnError != null)
                        OnError(_ws, ex);

                    if (_ws != null && _ws.State == WebSocketState.Open)

                        await _ws.CloseAsync(WebSocketCloseStatus.Empty, ex.Message, CancellationToken.None);
                }
                finally
                {
                    if (!_isUserClose)
                        Close(_ws.CloseStatus.Value, _ws.CloseStatusDescription + netErr);
                }
            });

        }

        public bool Send(string message)
        {
            if (_ws.State != WebSocketState.Open)
                return false;

            Task.Run(async () =>
            {
                var replyMess = Encoding.UTF8.GetBytes(message);
                await _ws.SendAsync(new ArraySegment<byte>(replyMess), WebSocketMessageType.Text, true, CancellationToken.None);
            });

            return true;
        }

        public bool Send(byte[] bytes)
        {
            if (_ws.State != WebSocketState.Open)
                return false;

            Task.Run(async () =>
            {
                await _ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Binary, true, CancellationToken.None);
            });

            return true;
        }

        public void Close()
        {
            _isUserClose = true;
            Close(WebSocketCloseStatus.NormalClosure, "用户手动关闭");
        }

        public void Close(WebSocketCloseStatus closeStatus, string statusDescription)
        {
            Task.Run(async () =>
            {
                try
                {
                    await _ws.CloseAsync(closeStatus, statusDescription, CancellationToken.None);
                }
                catch (Exception ex)
                {

                }

                _ws.Abort();
                _ws.Dispose();

                if (OnClose != null)
                    OnClose(_ws, new EventArgs());
            });
        }
    }
}
