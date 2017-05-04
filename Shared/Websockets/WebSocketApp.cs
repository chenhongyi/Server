using Logging;
using Model;
using Shared.Serializers;
using SuperSocket.SocketBase;
using SuperSocket.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Websockets
{
    public class WebSocketApp : IDisposable
    {
        private static readonly ILogger Logger = LoggerFactory.GetLogger(nameof(WebSocketApp));
        private readonly Func<IWebSocketConnectionHandler> createConnectionHandler;
        private static readonly byte[] UncaughtHttpBytes =
            Encoding.Default.GetBytes("主循环中产生一个未捕获的异常!");
        IWsSerializer pserializer;
        ProtobufWsSerializer mserializer;
        private WebSocketServer _ws;
        private string address;
        private int port;
        private CancellationToken cancellationToken;
        private CancellationTokenSource cancellationTokenSource;
        private HttpListener httpListener;

        public WebSocketApp(Func<IWebSocketConnectionHandler> createConnectionHandler, string address = "127.0.0.1", int port = 5555)
        {
            this.createConnectionHandler = createConnectionHandler;
            this.mserializer = new ProtobufWsSerializer();
            this.pserializer = SerializerFactory.CreateSerializer();
            this.address = address;
            this.port = port;
            this.cancellationTokenSource = new CancellationTokenSource();
        }

        public void Dispose()
        {
            Logger.Debug("释放资源，清理内存");

            try
            {
                if (this.cancellationTokenSource != null && !this.cancellationTokenSource.IsCancellationRequested)
                {
                    this.cancellationTokenSource.Cancel();
                }

                if (this.httpListener != null && this.httpListener.IsListening)
                {
                    this.httpListener.Stop();
                    this.httpListener.Close();
                }

                if (this.cancellationTokenSource != null && !this.cancellationTokenSource.IsCancellationRequested)
                {
                    this.cancellationTokenSource.Dispose();
                }
            }
            catch (ObjectDisposedException)
            {
            }
            catch (AggregateException ae)
            {
                ae.Handle(
                    ex =>
                    {
                        Logger.Error(ex, nameof(this.Dispose));
                        return true;
                    });
            }
        }

        public void Init()
        {
            if (!this.address.EndsWith("/"))
            {
                this.address += "/";
            }
            this._ws = new WebSocketServer();
            _ws.NewSessionConnected += OnClientConnected;
            _ws.NewMessageReceived += OnMessageReceived;
            _ws.NewDataReceived += OnDataReceived;
            _ws.SessionClosed += OnClientClosed;

            if (this._ws.Setup(this.port))
            {
                this._ws.Start();
            }
            //this.httpListener = new HttpListener();
            //this.httpListener.Prefixes.Add(this.address);
            this.cancellationTokenSource = new CancellationTokenSource();
            //this.cancellationToken = this.cancellationTokenSource.Token;
            //this.httpListener.Start();
        }


        /// <summary>
        /// 定时执行  主动去逻辑服务器取消息队列  拿回来进行发送
        /// </summary>
        /// <param name="session"></param>
        /// <param name="data"></param>
        public void SendToAllClients(WebSocketSession session)
        {
            byte[] data = null;
            foreach (var item in session.AppServer.GetAllSessions())
            {
                item.Send(data, 0, data.Length);
            }
        }


        /// <summary>
        /// 客户端断开
        /// </summary>
        /// <param name="session"></param>
        /// <param name="value"></param>
        public void OnClientClosed(WebSocketSession session, CloseReason value)
        {
            //TODO：客户端断开处理

        }

        /// <summary>
        /// 传入客户端数据
        /// </summary>
        /// <param name="session"></param>
        /// <param name="value"></param>
        public void OnDataReceived(WebSocketSession session, byte[] value)
        {
            IWebSocketConnectionHandler handler = this.createConnectionHandler();
            //收到数据处理 
            //对数据 value 进行 protobuf解包处理 验证sign  对于sign不正确的 直接返回  保留正确的包继续逻辑
            WsRequestMessage mrequest = mserializer.DeserializeAsync<WsRequestMessage>(value).Result;
            //if (!mrequest.SignKey.Equals(CheckSign(sign)))
            //{
            //TODO 验证处理
            // return  null;
            //}
            byte[] wsresponse = null;
            try
            {
                // dispatch to App provided function with requested
                wsresponse = handler.ProcessWsMessageAsync(value, session.SessionID, cancellationToken).Result;
            }
            catch (Exception ex)
            {
                // catch any error in the appAction and notify the client
                wsresponse = new ProtobufWsSerializer().SerializeAsync(
                    new WsResponseMessage
                    {
                        MsgId = 0,
                        Result = WsResult.Error,
                        Value = Encoding.UTF8.GetBytes(ex.Message)
                    }).Result;
            }
            session.Send(wsresponse, 0, wsresponse.Length);
            //TODO: 收到二进制数据处理
        }

        /// <summary>
        /// 收到客户端消息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="value"></param>

        public void OnMessageReceived(WebSocketSession session, string value)
        {
            byte[] wsresponse = null;
            wsresponse = new ProtobufWsSerializer().SerializeAsync(
                    new WsResponseMessage
                    {
                        MsgId = 0,
                        Result = WsResult.Success,
                        Value = Encoding.UTF8.GetBytes("测试文本消息")
                    }).Result;
            session.Send(wsresponse, 0, wsresponse.Length);

            //TODO: 收到文本消息处理
        }


        /// <summary>
        /// 客户端连接
        /// </summary>
        /// <param name="session"></param>
        public void OnClientConnected(WebSocketSession session)
        {
            //byte[] wsresponse = null;
            //wsresponse = new ProtobufWsSerializer().SerializeAsync(
            //        new WsResponseMessage
            //        {
            //            Result = WsResult.Success,
            //            Value = null
            //        }).Result;
            //session.Send(wsresponse, 0, wsresponse.Length);
            //TODO 客户端连接处理
            //预处理
        }

        /// <summary>
        /// 消息分发处理线程
        /// </summary>
        /// <param name="processActionAsync">回调函数</param>
        /// <returns></returns>
        public async Task StartAsync(Func<CancellationToken, HttpListenerContext, Task<bool>> processActionAsync)
        {
            //while (this.httpListener.IsListening)
            //{
            //    HttpListenerContext context = null;
            //    try
            //    {
            //        context = await this.httpListener.GetContextAsync();
            //        Logger.Debug("GetContextAsync complete");
            //    }
            //    catch (Exception ex)
            //    {
            //        // check if the exception is caused due to cancellation
            //        if (this.cancellationToken.IsCancellationRequested)
            //        {
            //            return;
            //        }

            //        Logger.Error(ex, "Error in GetContextAsync");
            //        continue;
            //    }
            //while (_ws.State == ServerState.Running)
            //{



            //    if (this.cancellationToken.IsCancellationRequested)
            //    {
            //        return;
            //    }


            //    this.DispatchConnectedContext(context, processActionAsync);
            //}
        }


        /// <summary>
        /// 新的连接已经建立，分发请求到回调函数 
        /// </summary>
        /// <param name="context">连接用户</param>
        /// <param name="processActionAsync">处理函数</param>
        private void DispatchConnectedContext(HttpListenerContext context, Func<CancellationToken, HttpListenerContext, Task<bool>> processActionAsync)
        {
            // do not await on processAction since we don't want to block on waiting for more connections
            processActionAsync(this.cancellationToken, context)
                .ContinueWith(
                    t =>
                    {
                        if (t.IsFaulted)
                        {
                            Logger.Error(t.Exception, "processAction did not handle their exceptions");
                            try
                            {
                                context.Response.ContentLength64 = UncaughtHttpBytes.Length;
                                context.Response.StatusCode = 500;
                                context.Response.OutputStream.Write(UncaughtHttpBytes, 0, UncaughtHttpBytes.Length);
                                context.Response.OutputStream.Close();
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex, "Couldn't write the 500 for misbehaving user");
                            }
                        }
                    },
                    this.cancellationToken);
        }
    }
}
