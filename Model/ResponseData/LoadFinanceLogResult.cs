﻿using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ResponseData
{
    [ProtoContract]
    public class LoadFinanceLogResult : BaseResponseData
    {
        [ProtoMember(1)] public List<LoadFinanceLogInfo> FinanceLog { get; set; } = new List<LoadFinanceLogInfo>();
    }

   
}
