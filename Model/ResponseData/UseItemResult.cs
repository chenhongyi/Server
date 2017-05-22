using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ResponseData
{
    /// <summary>
    /// 使用物品返回
    /// </summary>
    [ProtoContract]
    public class UseItemResult : BaseResponseData
    {
        [ProtoMember(1)] public BagInfo BagInfo { get; set; } = new BagInfo();
        [ProtoMember(2)] public List<UserAttr> ChangeAttr { get; set; } = new List<UserAttr>();  //可能改变的角色数值
        [ProtoMember(3)] public int Shenjia { get; set; }       //改变后的身价
        [ProtoMember(4)] public int Level { get; set; }         //改变后的级别
        [ProtoMember(5)] public long Exp { get; set; }          //改变后的经验值
    }

}
