using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Model.Protocol
{
    /// <summary>
    /// 客户端请求用户的房间信息
    /// </summary>
    [ProtoContract]
    public class RoomReq
    {
        [ProtoMember(1)] public string Id { get; set; }
    }

    /// <summary>
    /// 返回给客户端的房间信息
    /// </summary>
    [ProtoContract]
    public class RoomResult : BaseResponseData
    {
        [ProtoMember(1)] public string Id { get; set; }
        [ProtoMember(2)] public int RoomId { get; set; }
        [ProtoMember(3)] public List<RoomFurConfig> Config { get; set; }
    }

    [ProtoContract]
    public class RoomFurConfig
    {
        [ProtoMember(1)] public int key { get; set; }
        [ProtoMember(2)] public int itemId { get; set; }
        public RoomFurConfig(int key, int itemId)
        {
            this.key = key; this.itemId = itemId;
        }
    }
}