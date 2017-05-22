using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ResponseData
{
    [ProtoContract]
    public class GoldChangedResult
    {
        [ProtoMember(1)] public int GoldType { get; set; }
        [ProtoMember(2)] public long Count { get; set; }
    }
}
