using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ResponseData
{

    [ProtoContract]
    public class TCLevelUpResult
    {
        [ProtoMember(1)] public int NewLevel { get; set; }
        [ProtoMember(2)] public long NewExp { get; set; }
        [ProtoMember(3)] public List<UserAttr> RoleAttr { get; set; }
    }
}
