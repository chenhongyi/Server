using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.RequestData
{
    [ProtoContract]
    public class GetMapReq
    {
        [ProtoMember(1)]public int[] Pos { get; set; }
    }
}
