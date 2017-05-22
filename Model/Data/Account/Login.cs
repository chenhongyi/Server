using System;
using System.Runtime.Serialization;

namespace Model.Data.Account
{
    [DataContract]
    public class Login
    {
        [DataMember] public string Pid { get; set; }
        [DataMember] public Guid AccountId { get; set; }
    }
}
