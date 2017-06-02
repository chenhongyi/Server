using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.RequestData
{
    [ProtoContract]
    public class CreateBuildReq
    {
        [ProtoMember(1)] public int Pos { get; set; }
        [ProtoMember(2)] public string Name { get; set; }
        [ProtoMember(3)] public int ShopType { get; set; }
    }
}
