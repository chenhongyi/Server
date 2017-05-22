using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ResponseData
{
    /// <summary>
    /// 移除物品返回
    /// </summary>
    [ProtoContract]
    public class RemoveItemResult:BaseResponseData
    {
        [ProtoMember(1)] public BagInfo BagInfo { get; set; } = new BagInfo(); //背包数据
        [ProtoMember(2)] public long ShenJia { get; set; }  //身价改变
    }
}
