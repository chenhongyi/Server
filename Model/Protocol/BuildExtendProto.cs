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
    /// 店铺扩建
    /// </summary>
    [ProtoContract]
    public class BuildExtendReq
    {
        [ProtoMember(1)] public List<string> BuildId { get; set; }
    }
    /// <summary>
    /// 店铺扩建成功
    /// </summary>
    [ProtoContract]
    public class BuildExtendResult : BaseResponseData
    {
        [ProtoMember(1)] public List<LoadBuildInfo> BuildInfo = new List<LoadBuildInfo>();
    }
    /// <summary>
    /// 店铺扩建失败
    /// </summary>
    [ProtoContract]
    public class BuildExtendFailedResult : BaseResponseData
    {
        [ProtoMember(1)] public string BuildId { get; set; }
    }
}
