using Logging;
using LogicServer.Interface;
using Model;
using Model.MsgQueue;
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
    public class WebSocketApp
    {
        private static readonly ILogger Logger = LoggerFactory.GetLogger(nameof(WebSocketApp));
        private readonly Func<IWebSocketConnectionHandler> createConnectionHandler;
        private WebSocketServer _ws;
        private string address;
        private int port;
        private CancellationToken cancellationToken;
        private CancellationTokenSource cancellationTokenSource;
        IWebSocketConnectionHandler handler;
        private Timer _timerMsgQueue;
        private readonly ILogicServer _msgQueue;


        public WebSocketApp(Func<IWebSocketConnectionHandler> createConnectionHandler, string address = "127.0.0.1", int port = 5555)
        {

            this.createConnectionHandler = createConnectionHandler;
            handler = this.createConnectionHandler();
            this.address = address;
            this.port = port;
            this.cancellationTokenSource = new CancellationTokenSource();
            _msgQueue = ConnectionFactory.CreateLogicService();
            this._ws = new WebSocketServer();
            if (_ws.Setup(this.port))
            {
                _ws.Start();
            }
            _ws.NewSessionConnected += OnClientConnected;
            _ws.NewMessageReceived += OnMessageReceived;
            _ws.NewDataReceived += OnDataReceivedAsync;
            _ws.SessionClosed += OnClientClosed;
          //  _timerMsgQueue = new Timer(new TimerCallback(GetMsg), this, 0, 800);
        }



        public void Init()
        {
            if (!this.address.EndsWith("/"))
            {
                this.address += "/";
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
            SessionPool.Instance.OPool(session);
        }

        /// <summary>
        /// 传入客户端数据
        /// </summary>
        /// <param name="session"></param>
        /// <param name="value"></param>
        public async void OnDataReceivedAsync(WebSocketSession session, byte[] value)
        {

            //收到数据处理 
            //对数据 value 进行 protobuf解包处理 验证sign  对于sign不正确的 直接返回  保留正确的包继续逻辑
            WsRequestMessage mrequest = InitHelpers.GetMse().DeserializeAsync<WsRequestMessage>(value).Result;
            //if (!mrequest.SignKey.Equals(CheckSign(sign)))
            //{
            //TODO 验证处理
            // return  null;
            //}
            byte[] wsresponse = null;
            try
            {
                // dispatch to App provided function with requested
                //  wsresponse = handler.ProcessWsMessageAsync(value, session.SessionID, cancellationToken).Result;
                wsresponse = _msgQueue.ProcessWsMessageAsync1(value, session.SessionID, cancellationToken).Result;
            }
            catch (Exception ex)
            {
                // catch any error in the appAction and notify the client
                wsresponse = new ProtobufWsSerializer().SerializeAsync(
                    new WsResponseMessage
                    {
                        MsgId = 0,
                        Result = (int)WsResult.Error,
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
            handler.ProcessWsMessageAsync(value, cancellationToken);
        }


        /// <summary>
        /// 客户端连接
        /// </summary>
        /// <param name="session"></param>
        public void OnClientConnected(WebSocketSession session)
        {
            SessionPool.Instance.IPool(session);

        }



        public static void SendMsgToAllClients(MsgQueueList msg)
        {
            try
            {
                var data = msg.Data;
                var sessions = SessionPool.Instance.GetAll();
                if (sessions != null)
                {
                    foreach (var sid in sessions.Values)
                    {
                        if (sid != null)
                        {
                            if (sid.Connected)
                            {
                                sid.Send(data, 0, data.Length);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        public static void SendMsgToClients(MsgQueueList msg)
        {
            try
            {
                var data = msg.Data;
                var sessions = SessionPool.Instance.GetAll();
                if (sessions == null)
                {
                    return;
                }
                if (msg.Roles.Any())
                {
                    foreach (var session in sessions.Values)
                    {
                        if (session != null)
                        {
                            if (session.Connected)
                            {
                                foreach (var roleSession in msg.Roles)
                                {
                                    if (roleSession == session.SessionID)
                                    {
                                        session.Send(data, 0, data.Length);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        public static void SendMsgToSignleClient(MsgQueueList msg)
        {
            try
            {
                var data = msg.Data;
                if (msg == null)
                {
                    return;
                }
                if (msg.Roles.Any())
                {
                    foreach (var item in msg.Roles)
                    {
                        var session = SessionPool.Instance.GetSid(item);
                        //  var sid = this._ws.GetSessionByID(item);
                        if (session != null)
                        {
                            if (session.Connected)
                            {
                                session.Send(data, 0, data.Length);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }


    }
}
