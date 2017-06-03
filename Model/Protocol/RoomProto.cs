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
        [ProtoMember(1)] public string RoleId { get; set; }
    }

    /// <summary>
    /// 返回给客户端的房间信息
    /// </summary>
    [ProtoContract]
    public class RoomResult : BaseResponseData
    {
        [ProtoMember(1)] public string RoleId { get; set; }
        [ProtoMember(2)] public int RoomId { get; set; }
        [ProtoMember(3)] public Dictionary<int, int> Config { get; set; }
    }

    /// <summary>
    /// 请求更新房间配置
    /// </summary>
    [ProtoContract]
    public class RoomConfigUpdateReq
    {
        [ProtoMember(3)] public Dictionary<int, int> Config { get; set; }
    }

    /// <summary>
    /// 请求更新房间配置
    /// </summary>
    [ProtoContract]
    public class RoomConfigUpdateResult : BaseResponseData
    {
        [ProtoMember(3)] public Dictionary<int, int> Config { get; set; }
    }

    /// <summary>
    /// 房间购买/升级
    /// </summary>
    [ProtoContract]
    public class RoomBuyReq
    {
        [ProtoMember(1)] public int roomId { get; set; }
    }

    /// <summary>
    /// 房间购买/升级
    /// </summary>
    [ProtoContract]
    public class RoomBuyResult : BaseResponseData
    {
        [ProtoMember(1)] public int roomId { get; set; }
    }

    /// <summary>
    /// 房间出售
    /// </summary>
    [ProtoContract]
    public class RoomSellReq
    {

    }

    /// <summary>
    /// 房间出售
    /// </summary>
    [ProtoContract]
    public class RoomSellResult : BaseResponseData
    {

    }

    /// <summary>
    /// 拜访用户房间
    /// </summary>
    [ProtoContract]
    public class RoomVisitReq
    {
        [ProtoMember(1)] public string RoleId { get; set; }
    }

    /// <summary>
    /// 拜访用户房间
    /// </summary>
    [ProtoContract]
    public class RoomVisitResult : BaseResponseData
    {
        [ProtoMember(1)] public RoomResult Room { get; set; }
    }
}