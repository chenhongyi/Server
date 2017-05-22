using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ResponseData
{
    [ProtoContract]
    public class ChangeAvatarResult : BaseResponseData
    {
        [ProtoMember(1)] public BagInfo BagInfo { get; set; } = new BagInfo();
        [ProtoMember(2)] public List<UserAttr> ChangeAttr { get; set; } = new List<UserAttr>();  //可能改变的角色数值
    }
}
