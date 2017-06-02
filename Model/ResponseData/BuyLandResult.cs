using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ResponseData
{
    [ProtoContract]
    public class BuyLandResult:BaseResponseData
    {
        [ProtoMember(1)] public int State { get; set; }

        /// <summary>
        /// x坐标
        /// </summary>
        [ProtoMember(2)] public int PosX { get; set; }
        /// <summary>
        /// y坐标
        /// </summary>
        [ProtoMember(3)] public int PoxY { get; set; }

        [ProtoMember(4)] public string RoleId { get; set; }

    }
}
