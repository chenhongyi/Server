﻿using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ResponseData
{
    [ProtoContract]
    public class UpdateShenjiaResult:BaseResponseData
    {
        [ProtoMember(1)] public long SocialStatus { get; set; }
    }
}
