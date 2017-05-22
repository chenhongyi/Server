using Model.Data.Business;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.RequestData
{
    [ProtoContract]
    public class DepartmentUpdateReq
    {
        [ProtoMember(1)] public int Type { get; set; }
    }
}
