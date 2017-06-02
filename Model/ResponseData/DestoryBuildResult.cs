using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ResponseData
{
    [ProtoContract]
    public class DestoryBuildResult : BaseResponseData
    {
        [ProtoMember(1)] public LoadLandInfo LandInfo { get; set; } = new LoadLandInfo();
    }
}
