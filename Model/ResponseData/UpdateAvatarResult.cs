using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ResponseData
{
    [ProtoContract]
    public class UpdateAvatarResult:BaseResponseData
    {
        [ProtoMember(1)] public List<int> Id { get; set; } = new List<int>();
    }
}
