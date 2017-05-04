using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.RequestData
{
    [ProtoContract]
    public class ConnectingReq
    {
        [ProtoMember(1)]
        public string Token { get; set; }
    }
}
