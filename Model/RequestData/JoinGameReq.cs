using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.RequestData
{
    /// <summary>
    /// 客户端请求加入游戏
    /// </summary>
    [ProtoContract]
    public class JoinGameReq
    {
        [ProtoMember(1)] public string RoleId { get; set; }
    }
}
