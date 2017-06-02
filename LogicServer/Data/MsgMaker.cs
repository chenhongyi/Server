using LogicServer;
using LogicServer.Data.Helper;
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
        static IPublicGate gate = ConnectionFactory.CreatePublicGateService();
        public static async Task<byte[]> Make(WSResponseMsgID msgId, byte[] data)
        {
            WsResponseMessage result = new WsResponseMessage();
            result.MsgId = (int)msgId;
            result.Result = 0;
            result.Value = data;
            return await InitHelpers.GetMse().SerializeAsync(result);
        }

        public static async Task SendMessage(WSResponseMsgID msgId, int roleCount, byte[] data)
        {
            MsgQueueList msg = new MsgQueueList();
            var roleId = LogicServer.User.role.Id;
            msg.MsgType = msgId;
            msg.RoleCount = roleCount;

            msg.Roles.Add(await SidRoleIdDataHelper.Instance.GetSidByRoleIdAsync(roleId));
            msg.Data = await Make(msgId, data);
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
