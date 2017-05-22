using Model.Data.Npc;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ResponseData
{
    [ProtoContract]
    public class ConnectingResult : BaseResponseData
    {
        [ProtoMember(1)] public List<RoleLists> RoleLists { get; set; }
    }
    [ProtoContract]
    public class RoleLists
    {
        [ProtoMember(1)] public string RoleId { get; set; }
        [ProtoMember(2)] public string Name { get; set; }
        [ProtoMember(3)] public int Sex { get; set; }
    }
}
