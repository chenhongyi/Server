using LogicServer;
using Microsoft.ServiceFabric.Data;
using Model;
using Model.MsgQueue;
using Model.ResponseData;
using PublicGate.Interface;
using Shared;
using Shared.Serializers;
using SuperSocket.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicServer.Data
{
    public static class MsgMaker
    {
        static IPublicGate  gate = ConnectionFactory.CreatePublicGateService();
        public static async Task<byte[]> Make(WSResponseMsgID msgId, byte[] data)
        {
            WsResponseMessage result = new WsResponseMessage();
            result.MsgId = (int)msgId;
            result.Result = 0;
            result.Value = data;
            return await InitHelpers.GetMse().SerializeAsync(result);
        }

        public static async Task SendMessage(WSResponseMsgID msgId, int roleCount, Guid roleId, IReliableStateManager sm, byte[] data )
        {
            MsgQueueList msg = new MsgQueueList();
            msg.MsgType = msgId;
            msg.RoleCount = roleCount;
            msg.Roles.Add(await DataHelper.GetOnlineSessionByRoleIdAsync(sm, roleId));
            msg.Data = await MsgMaker.Make(msgId, data);
            switch (msg.RoleCount)
            {
                case 1: //一人
                    await gate.SendOne(msg);
                    break;
                case 0: //全体
                    await gate.SendAll(msg);
                    break;
                default:
                    break;
            }
        }
    }
}
