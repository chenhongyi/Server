using ProtoBuf;
using System.Collections.Generic;

namespace Model.Protocol
{
    /// <summary>
    /// 道具购买
    /// </summary>
    [ProtoContract]
    public class ItemBuyReq
    {
        [ProtoMember(1)] public int ItemId { get; set; }
        [ProtoMember(2)] public int Count { get; set; }
    }

    /// <summary>
    /// 道具购买返回
    /// </summary>
    [ProtoContract]
    public class ItemBuyResult
    {

    }

    /// <summary>
    /// 道具更新
    /// </summary>
    [ProtoContract]
    public class UpdateItemResult : BaseResponseData
    {
        [ProtoMember(1)] public List<ItemInfoCls> Items { get; set; }
    }

    [ProtoContract]
    public class ItemInfoCls
    {
        [ProtoMember(1)] public int ItemId { get; set; }
        [ProtoMember(2)] public long Count { get; set; }
    }
}