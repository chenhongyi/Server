using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ResponseData
{
    [ProtoContract]
    public class CreateBuildResult : BaseResponseData
    {
        [ProtoMember(1)] public string Id { get; set; }
        [ProtoMember(2)] public int ShopType { get; set; }
        [ProtoMember(3)] public string Name { get; set; }
        [ProtoMember(4)] public int TodayCanAdvartise { get; set; }
        [ProtoMember(5)] public int Level { get; set; }
        [ProtoMember(6)] public int Popularity { get; set; }
        [ProtoMember(7)] public int Star { get; set; }
        [ProtoMember(8)] public int Employee { get; set; }
        [ProtoMember(9)] public long GetMoney { get; set; }
    }
}
