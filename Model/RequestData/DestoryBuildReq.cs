using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.RequestData
{
    [ProtoContract]
    public class DestoryBuildReq
    {
        [ProtoMember(1)] public string Id { get; set; }
    }
}
