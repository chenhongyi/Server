﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model.Data.Business
{
    [DataContract]
    public class Employee
    {
        [DataMember] public Guid Id { get; set; }
    }

    public class Npc
    {

    }
}
