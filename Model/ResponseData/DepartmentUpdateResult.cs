using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ResponseData
{
    [ProtoContract]
    public class DepartmentUpdateResult : BaseResponseData
    {
        [ProtoMember(1)] public int Level { get; set; }

        [ProtoMember(2)] public int Type { get; set; }
    }
}
