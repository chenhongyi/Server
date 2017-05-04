using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicGate.Interface
{
    public interface IPublicGate : IService
    {

        /// <summary>
        /// 发送给所有用户广播
        /// </summary>
        /// <returns></returns>
        Task SendMsgToAllClients();

        /// <summary>
        /// 给部分用户广播
        /// </summary>
        /// <returns></returns>
        Task SendMsgToClients();

        /// <summary>
        /// 发送给单一用户消息
        /// </summary>
        /// <returns></returns>
        Task SendMsgToSignleClient();
    }
}
