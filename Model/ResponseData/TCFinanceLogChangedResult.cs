using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ResponseData
{
    [ProtoContract]
    public class TCFinanceLogChangedResult
    {
        [ProtoMember(1)] public LoadFinanceLogInfo FinanceLogInfo { get; set; } = new LoadFinanceLogInfo();
    }
}
