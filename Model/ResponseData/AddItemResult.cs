using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ResponseData
{
    /// <summary>
    /// 添加道具进背包返回
    /// </summary>
    [ProtoContract]
    public class AddItemResult : BaseResponseData
    {
        [ProtoMember(1)] public BagInfo BagInfo { get; set; } = new BagInfo(); //背包数据
        [ProtoMember(2)] public CanChangeAttr ChangeAttr { get; set; } = new CanChangeAttr(); //可能改变的角色数值
        [ProtoMember(3)] public long ShenJia { get; set; }  //身价改变
    }
}
