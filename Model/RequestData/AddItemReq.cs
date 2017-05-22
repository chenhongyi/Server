using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.RequestData
{    /// <summary>
     /// 添加道具进背包请求
     /// </summary>
    [ProtoContract]
    public class AddItemReq
    {
        [ProtoMember(1)] public int ItemId { get; set; }
        [ProtoMember(2)] public int Count { get; set; }
    }
}
