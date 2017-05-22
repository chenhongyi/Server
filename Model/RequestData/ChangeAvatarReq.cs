using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.RequestData
{
    [ProtoContract]
    public class ChangeAvatarReq
    {
        /// <summary>
        /// 时装id
        /// </summary>
        [ProtoMember(1)] public int[] Id { get; set; }
    }
}
