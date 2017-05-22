using System;
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

    [DataContract]
    public class Npc
    {
        [DataMember] public Guid Id { get; set; }
        [DataMember] public EmployeeType EmployeeType { get; set; }

    }

    [DataContract]
    public enum EmployeeType
    {
        [EnumMember] Normal = 1,    //普通员工
        [EnumMember] Manager = 2    //主管员工
    }
}
