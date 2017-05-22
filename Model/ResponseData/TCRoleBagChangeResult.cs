using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ResponseData
{
    [ProtoContract]
    public class TCRoleBagChangeResult : BaseResponseData
    {
        [ProtoMember(1)] public BagInfo BagInfo { get; set; }

    }
}
