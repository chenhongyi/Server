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
    /// 建筑升级
    /// </summary>
    [ProtoContract]
    public class BuildLvUpReq
    {
        [ProtoMember(1)] public List<string> BuildId { get; set; }
    }
    [ProtoContract]
    public class BuildLvUpResult : BaseResponseData
    {
        [ProtoMember(1)] public List<LoadBuildInfo> LandBuildInfo { get; set; } = new List<LoadBuildInfo>();
    }
    public class BuildLvUpFailedResult : BaseResponseData
    {
        [ProtoMember(1)] public string BuildId { get; set; }
    }
}
