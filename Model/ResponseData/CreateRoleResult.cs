using Model.Data.Npc;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ResponseData
{
    /// <summary>
    /// 客户端的请求：创建新角色 返回值
    /// </summary>
    [ProtoContract]
    public class CreateRoleResult : BaseResponseData
    {

        [ProtoMember(1)]        public string ErrorDesc { get; set; }
        [ProtoMember(2)]        public string RoleId { get; set; }
        [ProtoMember(3)]        public int Sex { get; set; }
        [ProtoMember(4)]        public string Name { get; set; }

    }


}
