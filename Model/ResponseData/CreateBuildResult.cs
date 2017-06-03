using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ResponseData
{
    [ProtoContract]
    public class CreateBuildResult : BaseResponseData
    {
        [ProtoMember(1)] public LoadBuildInfo LandBuildInfo { get; set; } = new LoadBuildInfo();
    }
}
