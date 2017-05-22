using ProtoBuf;
using SuperSocket.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model.MsgQueue
{
    [ProtoContract]
    [DataContract]
    public class MsgQueueList
    {
        /// <summary>
        /// 消息下发的用户列表
        /// </summary>
        [ProtoMember(1)] [DataMember] public List<string> Roles { get; set; } = new List<string>();
        /// <summary>
        /// //消息类型 根据消息类型分别决定如何序列化以及反序列化
        /// </summary>
        [ProtoMember(2)] [DataMember] public WSResponseMsgID MsgType { get; set; }
        /// <summary>
        ///  //消息体
        /// </summary>
        [ProtoMember(3)] [DataMember] public byte[] Data { get; set; }
        /// <summary>
        /// 发送人数 1-1人  xxx-多人  0-全体用户
        /// </summary>
        [ProtoMember(4)] [DataMember] public int RoleCount { get; set; }
    }
}
