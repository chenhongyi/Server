using Model.ResponseData;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Protocol
{
    /// <summary>
    /// 店铺信息更新消息
    /// </summary>
    [ProtoContract]
    public class TCBuildUpdateResult
    {
        [ProtoMember(1)] public LoadBuildInfo LandBuildInfo { get; set; } 
    }
}
