using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class ServiceConst
    {
        public const string DataApiWebsockets = "data";
        public const string ListenerRemoting = "Remoting1";
        public const string ListenerWebsocket = "Websocket1";
        public const string ListenerOwin = "Owin1";
    }



    /// <summary>
    /// 消息定义
    /// </summary>
    public class WSMsgID
    {
        public const int Connecting = 1000; //请求连接服务器
        public const int CreateRole = 1001; //创建角色
        public const int JoinGame = 1002;   //进入游戏
        public const int DelRole = 1003;    //删除角色

        //public const int 
    }
}
