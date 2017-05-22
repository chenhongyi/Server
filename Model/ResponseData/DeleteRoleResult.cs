using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ResponseData
{
    [ProtoContract]
    public class DeleteRoleResult
    {
        [ProtoMember(1)] public int State { get; set; }
    }
}
