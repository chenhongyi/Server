using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.RequestData
{
    [ProtoContract]
    public class RemoveItemReq
    {
        [ProtoMember(1)] public int ItemId { get; set; }
        [ProtoMember(2)] public int Count { get; set; }
    }
}
